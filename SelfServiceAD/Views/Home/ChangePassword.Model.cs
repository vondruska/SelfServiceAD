namespace SelfServiceAD.Views.Home
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Helpers;

    public class ChangePasswordModel
    {
        public ChangePasswordModel()
        {
            Username = WebsiteUser.Username;
        }

        public string Username { get; set; }

        [Required]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new passwords do not match")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}