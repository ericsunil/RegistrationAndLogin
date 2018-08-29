using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RegistrationAndLogin.Models
{
    public class UserLogin
    {

        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address is Required")]
        public string EmailID { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        //[MinLength(6, ErrorMessage = "Minimum 6 Characters Required")]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
    
}