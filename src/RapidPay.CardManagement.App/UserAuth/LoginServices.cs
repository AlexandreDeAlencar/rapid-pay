using ErrorOr;
using Microsoft.AspNetCore.Identity;
using RapidPay.CardManagement.App.UserLogin;

namespace RapidPay.CardManagement.App.Login
{
    public interface ILoginServices
    {
        Task<ErrorOr<string>> LoginAsync(string username, string password);
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginServices : ILoginServices
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenServices _tokenService;

        public LoginServices(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenServices tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ErrorOr<string>> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Error.Validation("User.NotFound", "User not found");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
            {
                return Error.Validation("User.InvalidPassword", "Password is incorrect");
            }

            var token = _tokenService.GenerateJwtToken(user.Id, user.UserName);
            return token;
        }
    }
}
