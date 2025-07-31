using Blog.Domain.Entities;

namespace Blog.API.Configuration;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            //Role-based policies
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(Role.RoleName.Admin));
            options.AddPolicy("UserOrAdmin", policy =>
                policy.RequireRole(Role.RoleName.User, Role.RoleName.Admin));

            //JWT-specific policies
            options.AddPolicy("ValidJwtToken", policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim("userId"));

            //Active user policy
            options.AddPolicy("ActiveUser", policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim("isActive", "True"));

            //Combined policies
            options.AddPolicy("ActiveAdmin", policy => 
               policy.RequireRole(Role.RoleName.Admin)
                    .RequireClaim("isActive", "True"));

        });
        return services;
    }
}
