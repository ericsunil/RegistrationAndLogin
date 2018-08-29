using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RegistrationAndLogin.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        //additional field most be added here which is not initially i.e. not save in db
        public string ConfirmPassword { get; set; }

    }
    public class UserMetadata
    {
        [DisplayName("First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "last Name is Required")]
        public string LastName { get; set; }

        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address is Required")]
        public string EmailID { get; set; }

        [DisplayName("Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString ="0: MM/dd/yyyy")]
        public DateTime DoB { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage ="Minimum 6 Characters Required")]
        public string Password { get; set; }

        [DisplayName("Conform Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 Characters Required")]
        [Compare("Password", ErrorMessage ="Conform Password and Password doesn't Match")]
        public string ConfirmPassword { get; set; }
    }
}