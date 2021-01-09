using System.Collections.Generic;

namespace Blaster.Shared.User
{
    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }
        
        public string Email { get; set; }

        public Dictionary<string, string> ExposedClaims { get; set; }
    }
}
