using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, RefreshToken refreshToken)> RefreshAsync(string refreshToken, string ip);
        Task RevokeAsync(string refreshToken, string ip);
        Task<(string accessToken, RefreshToken refreshToken)> SignInAsync(string usernameOrEmail, string password, string ip);
    }
}