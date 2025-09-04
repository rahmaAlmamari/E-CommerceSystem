using E_CommerceSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace E_CommerceSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwt;

        public AuthService(ApplicationDbContext db, ITokenService tokenService, IOptions<JwtSettings> jwtOptions)
        {
            _db = db;
            _tokenService = tokenService;
            _jwt = jwtOptions.Value;
        }

        public async Task<(string accessToken, RefreshToken refreshToken)> SignInAsync(string usernameOrEmail, string password, string ip)
        {
            var user = await _db.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == usernameOrEmail || u.UName == usernameOrEmail);

            if (user == null) throw new UnauthorizedAccessException("Invalid credentials.");

            // TODO: replace with your password-hash verification
            // === Modified: verify hashed password (BCrypt) and migrate any plain-text to BCrypt ===
            var stored = user.Password ?? string.Empty;
            bool ok;

            // If stored looks like a BCrypt hash ($2a$ / $2b$ / $2y$), verify with BCrypt
            if (stored.StartsWith("$2a$") || stored.StartsWith("$2b$") || stored.StartsWith("$2y$"))
            {
                ok = BCrypt.Net.BCrypt.Verify(password, stored);
            }
            else
            {
                // Fallback: if some old rows were plain-text during early dev
                ok = stored == password;

                // If it matched as plain text, immediately upgrade to BCrypt (no extra SaveChanges here;
                // your method calls SaveChangesAsync below after issuing tokens)
                if (ok)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
                }
            }

            if (!ok) throw new UnauthorizedAccessException("Invalid credentials.");

            var access = _tokenService.CreateAccessToken(user);
            var refresh = _tokenService.CreateRefreshToken(ip, _jwt.RefreshTokenDays);

            user.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            return (access, refresh);
        }

        public async Task<(string accessToken, RefreshToken refreshToken)> RefreshAsync(string refreshToken, string ip)
        {
            var token = await _db.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null) throw new UnauthorizedAccessException("Invalid token.");
            if (!token.IsActive) throw new UnauthorizedAccessException("Token is not active.");

            // rotate
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ip;

            var newRefresh = _tokenService.CreateRefreshToken(ip, _jwt.RefreshTokenDays);
            token.ReplacedByToken = newRefresh.Token;

            token.User.RefreshTokens.Add(newRefresh);
            var newAccess = _tokenService.CreateAccessToken(token.User);

            await _db.SaveChangesAsync();
            return (newAccess, newRefresh);
        }

        public async Task RevokeAsync(string refreshToken, string ip)
        {
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (token == null) return;

            if (token.Revoked == null)
            {
                token.Revoked = DateTime.UtcNow;
                token.RevokedByIp = ip;
                await _db.SaveChangesAsync();
            }
        }
    }
}
