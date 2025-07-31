using Blog.Domain.Entities;

namespace Blog.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user, IList<string> roles);
    string GenerateRefreshToken(User user);
    bool ValidateRefreshToken(string refreshToken);
    int? GetUserIdFromExpiredToken(string expiredToken);
}
