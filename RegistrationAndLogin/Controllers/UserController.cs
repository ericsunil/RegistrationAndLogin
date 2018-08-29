using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RegistrationAndLogin.Controllers
{
    public class UserController : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //Registration Post Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified, ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";
            //Model Validation
            if (ModelState.IsValid)
            {

                #region //Email is Already exist or not 
                var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already Exist");
                    return View(user);
                }
                #endregion
                #region //Generate Activation Code
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region //password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion
                user.IsEmailVerified = false;

                #region //Save data to db
                using ( DBModel dc = new DBModel())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();

                    //send email to user
                    SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                    message = "Registration Successfully Done. Account activation link " + " has been sent to your email address: " + user.EmailID;
                    Status = true;

                }
                #endregion

            }
            else
            {
                message = "Invalid Request";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        //Verify Account
        [HttpGet]
        public ActionResult VerifyAccount(string  id)//get activation code from the link sent to the user
        {
            bool Status = false;
            using (DBModel dc = new DBModel())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;//added to avoid conform password doesn't match issue on save changes
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if(v != null)//mean the link is valid
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
                return View();
        }

        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //Login Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl)
        {
            String message = "";
            using ( DBModel dc = new DBModel())
            {
                var v = dc.Users.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                if(v != null)
                {
                    if(String.Compare(Crypto.Hash(login.Password), v.Password ) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; //if remember me then it should be stored for 1 yr ie 525600 min, else save for 20min
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true; //if doesn't want toacess it from JS.
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid Credental Provided";
                    }
                }
                else
                {
                    message = "Invalid Credental Provided";
                }
            }
                ViewBag.Message = message;
            return View();
        }
        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
        [NonAction]
        public bool IsEmailExist(string emailID)
            {
                using (DBModel dc = new DBModel())
                {
                    var v = dc.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
                    return v != null;
                }
            }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationcode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("abinaskosansar@gmail.com", "Sunil Hyaunmikha");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "handsome";//
            string subject = "Your account is successfully created!";

            string body = "<br><br>Your account is" + " Successfully created. Please click on the below link to verify your account " + "<br><br><a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod= SmtpDeliveryMethod.Network,
                UseDefaultCredentials =  false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,

            })
                smtp.Send(message);
           
        }

    }
}