using Microsoft.Extensions.Configuration;
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

        public TokenServices(IConfiguration configuration)
        {
            _issuer = configuration["Jwt:Issuer"]
                ?? throw new ArgumentNullException(nameof(_issuer), "JWT Issuer configuration is missing.");
            _audience = configuration["Jwt:Audience"]
                ?? throw new ArgumentNullException(nameof(_audience), "JWT Audience configuration is missing.");
            _secretKey = configuration["Jwt:SecretKey"]
                ?? throw new ArgumentNullException(nameof(_secretKey), "JWT Secret Key configuration is missing.");
        }

        public string GenerateJwtToken(string userId, string userName)
        {
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
            return tokenHandler.WriteToken(token);
        }
    }
}
