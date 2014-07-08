using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SelfServiceAD.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.ModelBinding;

    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel()
        {
            Username = (string)HttpContext.Current.Session["Username"];
        }
        public string Username { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}