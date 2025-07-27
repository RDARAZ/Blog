using Blog.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Blog.Domain.Entities;

public class User : IdentityUser<int>
{
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public int? Age { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
