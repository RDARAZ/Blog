using Blog.Application.Interfaces;
using Blog.Application.Options;
using Blog.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Infrastructure.Services;

public class TokenService(IOptions<JwtSettings> jwtSettings, ILogger<TokenService> logger) : ITokenService
{
	private readonly JwtSettings _jwtSettings = jwtSettings.Value;

	public async Task<string> GenerateAccessTokenAsync(User user, IList<string> roles)
	{
		try
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new(ClaimTypes.Name, user.UserName!),
				new(ClaimTypes.Email, user.Email!),
				new("userId", user.Id.ToString()),
				new("isActive", user.IsActive.ToString())
			};

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
				SigningCredentials = credentials,
				Issuer = _jwtSettings.Issuer,
				Audience = _jwtSettings.Audience
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			logger.LogInformation("Access token generated for user {UserId}", user.Id);

			return tokenHandler.WriteToken(token);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error generating access token for user {UserId}", user.Id);
			throw;
		}
	}

    public string GenerateRefreshToken(User user)
    {
        try
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = Convert.ToBase64String(randomNumber);

            logger.LogInformation("Refresh token generated for user {UserId}", user.Id);

            return refreshToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating refresh token for user {UserId}", user.Id);
            throw;
        }
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        try
        {
            // Basic validation - check if it's a valid base64 string
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var bytes = Convert.FromBase64String(refreshToken);
            return bytes.Length == 64;
        }
        catch
        {
            return false;
        }
    }

    public int? GetUserIdFromExpiredToken(string expiredToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, // We want to accept expired tokens
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(expiredToken, validationParameters, out _);
            var userIdClaim = principal.FindFirst("userId")?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to extract user ID from expired token");
            return null;
        }
    }
}
