using System.ComponentModel.DataAnnotations;

namespace Blaster.Shared.User
{
    public class ResendConfirmEmailModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
