using Microsoft.AspNetCore.Identity;

namespace Blog.Domain.Entities;

public class Role : IdentityRole<int>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;

    public static class RoleName
    {
        public const string User = "User"; 
        public const string Admin = "Admin";
    }

    public Role() : base() { }

    public Role(string roleName) : base(roleName)
    {
        Name = roleName;
        NormalizedName = roleName.ToUpperInvariant();
    }
}
