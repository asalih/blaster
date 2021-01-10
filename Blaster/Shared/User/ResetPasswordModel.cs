using System.ComponentModel.DataAnnotations;

namespace Blaster.Shared.User
{
    public class ResetPasswordModel
    {
        [Required]
        public string ResetToken { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string PasswordConfirm { get; set; }
    }
}
