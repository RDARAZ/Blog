using Blog.Application.DTOs.Authentication;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandHandler(UserManager<User> userManager, ITokenService tokenService, ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
		try
		{
            logger.LogInformation("Processing registration for {UserName} with email {Email}",
                request.UserName, request.Email);

            var existingUsers = await userManager.Users
                .Where(u => u.Email == request.Email || u.UserName == request.UserName)
                .Select(u => new { u.Email, u.UserName })
                .ToListAsync(cancellationToken);

            if (existingUsers.Any(u => u.Email == request.Email))
            {
                logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
                return AuthResponse.Failed("A user with this email already exists.");
            }

            if (existingUsers.Any(u => u.UserName == request.UserName))
            {
                logger.LogWarning("Registration failed: Username {UserName} already exists", request.UserName);
                return AuthResponse.Failed("A user with this username already exists.");
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Gender = request.Gender,
                Age = request.Age,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, request.Password);

            if(!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description).ToArray();
                logger.LogWarning("Registration failed for {Email}: {Errors}", request.Email, string.Join(", ", errors));
                return AuthResponse.Failed(errors);
            }

            var roleResult = await userManager.AddToRoleAsync(user, Role.RoleName.User);

            if(!roleResult.Succeeded)
            {
                logger.LogWarning("Failed to assign role to user {UserId}", user.Id);
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = await tokenService.GenerateAccessTokenAsync(user, roles);
            var refreshToken = tokenService.GenerateRefreshToken(user);

            logger.LogInformation("User {UserId} registered successfully", user.Id);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Gender = user.Gender,
                Age = user.Age,
                IsActive = user.IsActive,
                Roles = [..roles]
            };

            return AuthResponse.Success(token, refreshToken, DateTime.UtcNow.AddHours(1), userDto);
        }
        catch (Exception ex)
		{
            logger.LogError(ex, "Error occurred during registration for email: {Email}", request.Email);
            return AuthResponse.Failed("An error occurred during registration. Please try again.");
        }
    }
}
