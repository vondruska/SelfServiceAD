using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SelfServiceAD.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}