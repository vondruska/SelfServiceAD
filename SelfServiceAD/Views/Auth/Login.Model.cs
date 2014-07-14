namespace SelfServiceAD.Views.Auth
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}