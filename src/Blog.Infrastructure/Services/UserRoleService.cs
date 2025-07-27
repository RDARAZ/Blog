using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Infrastructure.Services;

public class UserRoleService(UserManager<User> userManager) : IUserRoleService
{
    public async Task<bool> AddUserToRoleAsync(User user, string roleName)
    {
        var result = await userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> CanWriteArticlesAsync(User user)
    {
        var isAdmin = await IsAdminAsync(user);
        return user.IsActive && isAdmin;
    }

    public async Task<IList<string>> GetUserRolesAsync(User user)
    {
        return await userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsAdminAsync(User user)
    {
        return await userManager.IsInRoleAsync(user, Role.RoleName.Admin);
    }

    public async Task<bool> IsUserInRoleAsync(User user, string roleName)
    {
        return await userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<bool> RemoveUserFromRoleAsync(User user, string roleName)
    {
        var result = await userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }
}
