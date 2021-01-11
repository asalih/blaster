using Blaster.Infrastructure.Entity;
using Blaster.Infrastructure.Utility;
using Blaster.Infrastructure.Utility.Contracts;
using Blaster.Shared.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blaster.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizeController : AppControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailHelper _emailHelper;
        private readonly IDataProtector _dataProtector;
        private readonly CustomUrlHelper _customUrlHelper;
        private readonly string _errorMessage = "Invalid email address or password!";

        public AuthorizeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailHelper emailHelper,
            IDataProtectionProvider dataProtectionProvider,
            CustomUrlHelper customUrlHelper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailHelper = emailHelper;
            _dataProtector = dataProtectionProvider.CreateProtector("DataProtectorTokenProvider");
            _customUrlHelper = customUrlHelper;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Email);

            if (user == null)
            {
                return BadRequest(_errorMessage);
            }

            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

            if (!singInResult.Succeeded)
            {
                return BadRequest(_errorMessage);
            }

            await _signInManager.SignInAsync(user, loginModel.RememberMe);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var user = new User
            {
                UserName = registerModel.Email,
                Email = registerModel.Email
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            var createdUser = await _userManager.FindByEmailAsync(registerModel.Email);

            if(createdUser == null)
            {
                return BadRequest("Something went wrong!");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(createdUser);

            var uri = _customUrlHelper.GenerateUrl("confirm-email", new { t = token });
            _emailHelper.Render(to: user.Email, subject: "Confirm Email", template: EmailTemplate.ConfirmEmail, new { uri });

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
            {
                return Ok();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var uri = _customUrlHelper.GenerateUrl("reset-password", new { t = token });
            _emailHelper.Render(to: user.Email, subject: "Reset Password", template: EmailTemplate.ResetPassword, new { uri });

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            const string _error = "Reset password has been failed!";

            if (!Infrastructure.Extensions.TryFromBase64String(resetPasswordModel.ResetToken, out var resetTokenArray))
            {
                return BadRequest(_error);
            }
            
            var unprotectedResetTokenArray = _dataProtector.Unprotect(resetTokenArray);

            var userIdInput = string.Empty;

            using (var ms = new MemoryStream(unprotectedResetTokenArray))
            using (var reader = new BinaryReader(ms))
            {
                reader.ReadInt64();
                userIdInput = reader.ReadString();
            }

            if(!Guid.TryParse(userIdInput, out var _))
            {
                return BadRequest(_error);
            }

            var user = await _userManager.FindByIdAsync(userIdInput);

            if(user == null)
            {
                return BadRequest(_error);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.ResetToken, resetPasswordModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(_error);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel confirmEmailModel)
        {
            const string _error = "Email confirmation has been failed!";

            if (!Infrastructure.Extensions.TryFromBase64String(confirmEmailModel.ConfirmToken, out var confirmTokenArray))
            {
                return BadRequest(_error);
            }

            var unprotectedConfirmTokenArray = _dataProtector.Unprotect(confirmTokenArray);

            var userIdInput = string.Empty;

            using (var ms = new MemoryStream(unprotectedConfirmTokenArray))
            using (var reader = new BinaryReader(ms))
            {
                reader.ReadInt64();
                userIdInput = reader.ReadString();
            }

            if (!Guid.TryParse(userIdInput, out var _))
            {
                return BadRequest(_error);
            }

            var user = await _userManager.FindByIdAsync(userIdInput);

            if (user == null)
            {
                return BadRequest(_error);
            }

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailModel.ConfirmToken);

            if (!result.Succeeded)
            {
                return BadRequest(_error);
            }

            return Ok();
        }

        [HttpGet]
        public UserInfo UserInfo() => new UserInfo
        {
            IsAuthenticated = User.Identity.IsAuthenticated,
            Email = User.Identity.Name,
            ExposedClaims = User.Claims
                    //Optionally: filter the claims you want to expose to the client
                    //.Where(c => c.Type == "test-claim")
                    .ToDictionary(c => c.Type, c => c.Value)
        };
    }
}
