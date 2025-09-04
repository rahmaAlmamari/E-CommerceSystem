using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        RefreshToken CreateRefreshToken(string ipAddress, int daysToLive);
    }
}