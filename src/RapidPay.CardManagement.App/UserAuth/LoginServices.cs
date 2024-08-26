using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoginServices> _logger;

        public LoginServices(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ITokenServices tokenService,
            ILogger<LoginServices> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ErrorOr<string>> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Login attempt for user: {Username}", username);

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Username} not found.", username);
                return Error.Validation("User.NotFound", "User not found");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for user {Username}.", username);
                return Error.Validation("User.InvalidPassword", "Password is incorrect");
            }

            var token = _tokenService.GenerateJwtToken(user.Id, user.UserName);
            _logger.LogInformation("Login successful for user: {Username}. Token generated.", username);
            return token;
        }
    }
}
