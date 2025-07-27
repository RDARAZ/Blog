using Blog.Domain.Entities;

namespace Blog.API.Configuration;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(Role.RoleName.Admin));
            options.AddPolicy("UserOrAdmin", policy =>
                policy.RequireRole(Role.RoleName.User, Role.RoleName.Admin));
        });
        return services;
    }
}
