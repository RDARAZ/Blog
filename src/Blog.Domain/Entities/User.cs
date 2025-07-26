using Blog.Domain.Enums;

namespace Blog.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public int? Age { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    public bool IsAdmin() => Role == UserRole.Admin;
    public bool CanWriteArticles() => IsActive && IsAdmin();
}
