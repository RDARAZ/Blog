using Blog.Domain.Enums;

namespace Blog.Application.DTOs.Authentication;

public class RegisterDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public int? Age { get; set; }
}
