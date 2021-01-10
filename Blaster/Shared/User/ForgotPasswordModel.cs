using System.ComponentModel.DataAnnotations;

namespace Blaster.Shared.User
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
