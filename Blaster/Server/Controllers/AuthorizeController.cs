using Blaster.Infrastructure.Entity;
using Blaster.Shared.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Blaster.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly string _errorMessage = "Invalid email address or password!";

        public AuthorizeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
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

            return await Login(new LoginModel
            {
                Email = registerModel.Email,
                Password = registerModel.Password
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

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
