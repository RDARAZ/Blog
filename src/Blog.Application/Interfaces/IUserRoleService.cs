using Blog.Domain.Entities;

namespace Blog.Application.Interfaces;

public interface IUserRoleService
{
    Task<bool> IsUserInRoleAsync(User user, string roleName);
    Task<bool> IsAdminAsync(User user);
    Task<bool> CanWriteArticlesAsync(User user);
    Task<IList<string>> GetUserRolesAsync(User user);
    Task<bool> AddUserToRoleAsync(User user, string roleName);
    Task<bool> RemoveUserFromRoleAsync(User user, string roleName);
}
