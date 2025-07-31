using Blog.Application.DTOs.Authentication;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Blog.Application.Features.Authentication.Queries.LoginUser;

public class LoginUserQueryHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService,
    ILogger<LoginUserQueryHandler> logger) : IRequestHandler<LoginUserQuery, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing login request for email: {Email}", request.Email);

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                return AuthResponse.Failed("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                logger.LogWarning("Login failed: User {UserId} is inactive", user.Id);
                return AuthResponse.Failed("Your account has been deactivated. Please contact support.");
            }

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);


            if (signInResult.IsLockedOut)
            {
                logger.LogWarning("Login failed: User {UserId} is locked out", user.Id);
                return AuthResponse.Failed("Account is locked due to multiple failed login attempts. Please try again later.");
            }

            if (!signInResult.Succeeded)
            {
                logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                return AuthResponse.Failed("Invalid email or password.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = await tokenService.GenerateAccessTokenAsync(user, roles);
            var refreshToken = tokenService.GenerateRefreshToken(user);

            user.UpdatedAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
            logger.LogInformation("User {UserId} logged in successfully", user.Id);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Gender = user.Gender,
                Age = user.Age,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
            };

            var expiration = request.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1);
            return AuthResponse.Success(token, refreshToken, expiration, userDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during login for email: {Email}", request.Email);
            return AuthResponse.Failed("An error occurred during login. Please try again.");
        }
    }
}
