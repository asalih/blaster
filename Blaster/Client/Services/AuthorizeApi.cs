using Blaster.Client.Services.Contracts;
using Blaster.Shared.User;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blaster.Client.Services
{
    public class AuthorizeApi : IAuthorizeApi
    {
        private readonly HttpClient _httpClient;

        public AuthorizeApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserInfo> GetUserInfo() =>
            await _httpClient.GetFromJsonAsync<UserInfo>("api/Authorize/UserInfo");

        public async Task Login(LoginModel loginModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/Login", loginModel);

            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await result.Content.ReadAsStringAsync());
            }

            result.EnsureSuccessStatusCode();
        }

        public async Task Logout()
        {
            var result = await _httpClient.PostAsync("api/Authorize/Logout", null);

            result.EnsureSuccessStatusCode();
        }

        public async Task Register(RegisterModel registerModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/Register", registerModel);
            
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await result.Content.ReadAsStringAsync());
            }
            
            result.EnsureSuccessStatusCode();
        }

        public async Task ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/ForgotPassword", forgotPasswordModel);

            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await result.Content.ReadAsStringAsync());
            }

            result.EnsureSuccessStatusCode();
        }

        public async Task ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/ResetPassword", resetPasswordModel);

            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await result.Content.ReadAsStringAsync());
            }

            result.EnsureSuccessStatusCode();
        }

        public async Task ConfirmEmail(ConfirmEmailModel confirmEmailModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/ConfirmEmail", confirmEmailModel);

            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await result.Content.ReadAsStringAsync());
            }

            result.EnsureSuccessStatusCode();
        }
    }
}
