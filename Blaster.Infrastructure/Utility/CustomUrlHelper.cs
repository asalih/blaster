using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Web;

namespace Blaster.Infrastructure.Utility
{
    public class CustomUrlHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;

        private string _baseUrl => _configuration.GetSection("ServerUrl")?.Value;

        public CustomUrlHelper(
            IConfiguration configuration,
            IHttpContextAccessor accessor
            )
        {
            _configuration = configuration;
            _accessor = accessor;
        }

        public string GenerateUrl(string action) => $"{_baseUrl ?? _accessor?.HttpContext?.Request?.GetDisplayUrl()}/{action}";

        public string GenerateUrl(string action, object query)
        {
            var root = $"{_baseUrl ?? _accessor?.HttpContext?.Request?.GetDisplayUrl()}/{action}";

            var parameters = query?.ToDictionary();

            if (parameters != null)
            {
                root = $"{root}?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value.ToString())}"))}";
            }

            return root;
        }


    }
}
