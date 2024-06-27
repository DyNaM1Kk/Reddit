using Microsoft.IdentityModel.Tokens;
using Reddit.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Reddit.Services
{
    public class TokenManager
    {
        private readonly IConfiguration _config;

        public TokenManager(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user)
        {
            var expires = DateTime.UtcNow.AddHours(1);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["JwtSettings:Issuer"], _config["JwtSettings:Audience"], CreateClaims(user), expires: expires, signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var rand = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(rand);
                return Convert.ToBase64String(rand);
            }
        }

        private List<Claim> CreateClaims(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                return claims;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
