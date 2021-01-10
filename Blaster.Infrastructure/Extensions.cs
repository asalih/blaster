using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaster.Infrastructure
{
    public static class Extensions
    {
        public static Dictionary<string, object> ToDictionary(this object arg) =>
            arg?.GetType().GetProperties().ToDictionary(property => property.Name, property => property.GetValue(arg));

        public static bool TryFromBase64String(string input, out byte[] result)
        {
            result = null;

            try
            {
                result = Convert.FromBase64String(input);
                return true;
            }
            catch 
            {
                // noop    
            }

            return false;
        }
    }
}
