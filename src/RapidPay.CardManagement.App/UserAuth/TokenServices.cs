using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RapidPay.CardManagement.App.UserLogin
{
    public interface ITokenServices
    {
        string GenerateJwtToken(string userId, string userName);
    }

    public class TokenServices : ITokenServices
    {
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly string? _secretKey;
        private readonly ILogger<TokenServices> _logger;

        public TokenServices(IConfiguration configuration, ILogger<TokenServices> logger)
        {
            _issuer = configuration["JwtSettings:Issuer"]
                ?? throw new ArgumentNullException(nameof(_issuer), "JWT Issuer configuration is missing.");
            _audience = configuration["JwtSettings:Audience"]
                ?? throw new ArgumentNullException(nameof(_audience), "JWT Audience configuration is missing.");
            _secretKey = configuration["JwtSettings:SecretKey"]
                ?? throw new ArgumentNullException(nameof(_secretKey), "JWT Secret Key configuration is missing.");

            _logger = logger;
        }

        public string GenerateJwtToken(string userId, string userName)
        {
            _logger.LogInformation("Generating JWT token for user: {UserName}, UserId: {UserId}", userName, userId);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", userId),
                    new Claim("name", userName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _audience,
                Issuer = _issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("JWT token generated successfully for user: {UserName}, UserId: {UserId}", userName, userId);

            return tokenString;
        }
    }
}
