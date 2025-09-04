using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using E_CommerceSystem.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace E_CommerceSystem.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwt;
        public TokenService(IOptions<JwtSettings> jwtOptions) => _jwt = jwtOptions.Value;

        public string CreateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UID.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UID.ToString()),
                new Claim(ClaimTypes.Name, user.UName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            };
            if (!string.IsNullOrWhiteSpace(user.Email))
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken CreateRefreshToken(string ipAddress, int daysToLive)
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            var opaque = Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            var now = DateTime.UtcNow;
            return new RefreshToken
            {
                Token = opaque,
                Created = now,
                CreatedByIp = ipAddress ?? string.Empty,
                Expires = now.AddDays(daysToLive)
            };
        }
    }
}
