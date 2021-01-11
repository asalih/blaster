using System.ComponentModel.DataAnnotations;

namespace Blaster.Shared.User
{
    public class ConfirmEmailModel
    {
        [Required]
        public string ConfirmToken { get; set; }
    }
}
