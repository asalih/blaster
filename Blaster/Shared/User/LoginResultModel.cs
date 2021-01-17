namespace Blaster.Shared.User
{
    public class LoginResultModel
    {
        public bool IsNotAllowed { get; set; }
        
        public bool IsLockedOut { get; set; }

        public bool RequiresTwoFactor { get; set; }

        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; }
    }
}
