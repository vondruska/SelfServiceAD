using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SelfServiceAD.Models
{
    public class UserViewModel
    {
        public DateTime? PasswordExpiration { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? LastPasswordSet { get; set; }
        public string DisplayName { get; set; }
        public bool PasswordNeverExpires { get; set; }
    }
}