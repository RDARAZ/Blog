namespace Blog.Application.DTOs.Authentication;

public class AuthResponse
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime TokenExpiration { get; set; }
    public UserDto? User { get; set; }
    public List<string> Errors { get; set; } = new();

    public static AuthResponse Success(string token, string refreshToken, DateTime expiration, UserDto user)
    {
        return new AuthResponse
        {
            IsSuccess = true,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiration = expiration,
            User = user
        };
    }

    public static AuthResponse Failed(params string[] errors)
    {
        return new AuthResponse
        {
            IsSuccess = false,
            Errors = [.. errors]
        };
    }
}
