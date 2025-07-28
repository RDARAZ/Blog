using Blog.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Infrastructure.Data.DataSeeding;
public static class IdentityDataSeeder
{
    public static async Task SeedAsync(
        BlogDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        await EnsureRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
    }

    private static async Task EnsureRolesAsync(RoleManager<Role> roleManager)
    {
        var roles = new[]
        {
            new { Name = Role.RoleName.User, Description = "Standard user with basic permissions" },
            new { Name = Role.RoleName.Admin, Description = "Administrator with full permissions" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                var newRole = new Role(role.Name)
                {
                    Description = role.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await roleManager.CreateAsync(newRole);
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<User> userManager)
    {
        var adminEmail = "admin@blog.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                Age = 30,
                Gender = Domain.Enums.Gender.Male,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Role.RoleName.Admin);
            }
        }

        var userEmail = "user@blog.com";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var testUser = new User
            {
                UserName = "testuser",
                Email = userEmail,
                EmailConfirmed = true,
                Age = 25,
                Gender = Domain.Enums.Gender.Female,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(testUser, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, Role.RoleName.User);
            }
        }
    }
}
