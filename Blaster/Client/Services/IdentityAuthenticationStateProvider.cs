using Blaster.Client.Services.Contracts;
using Blaster.Shared.User;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blaster.Client.Services
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private UserInfo _userInfoCache;
        private readonly IAuthorizeApi _authorizeApi;

        public IdentityAuthenticationStateProvider(IAuthorizeApi authorizeApi)
        {
            _authorizeApi = authorizeApi ?? throw new ArgumentNullException(nameof(authorizeApi));
        }

        public async Task Login(LoginModel loginModel)
        {
            await _authorizeApi.Login(loginModel);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Register(RegisterModel registerModel)
        {
            await _authorizeApi.Register(registerModel);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout()
        {
            await _authorizeApi.Logout();

            _userInfoCache = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            await _authorizeApi.ForgotPassword(forgotPasswordModel);
        }

        public async Task ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            await _authorizeApi.ResetPassword(resetPasswordModel);
        }

        public async Task ConfirmEmail(ConfirmEmailModel confirmEmailModel)
        {
            await _authorizeApi.ConfirmEmail(confirmEmailModel);
        }

        private async Task<UserInfo> GetUserInfo()
        {
            if (_userInfoCache != null && _userInfoCache.IsAuthenticated)
            {
                return _userInfoCache;
            }
            
            _userInfoCache = await _authorizeApi.GetUserInfo();
            
            return _userInfoCache;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                var userInfo = await GetUserInfo();
                if (userInfo.IsAuthenticated)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, userInfo.Email) }.Concat(userInfo.ExposedClaims.Select(c => new Claim(c.Key, c.Value)));
                    identity = new ClaimsIdentity(claims, "Server authentication");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
