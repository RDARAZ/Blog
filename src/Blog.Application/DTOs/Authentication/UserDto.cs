using Blog.Domain.Enums;

namespace Blog.Application.DTOs.Authentication;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int? Age { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}
