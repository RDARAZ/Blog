using Blog.Application.DTOs.Authentication;
using Blog.Application.Features.Authentication.Commands.RegisterUser;
using Blog.Application.Features.Authentication.Queries.LoginUser;
using Blog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (request is null)
        {
            logger.LogWarning("Register request is null");
            return BadRequest(AuthResponse.Failed("Invalid request data."));
        }
		try
		{
			var command = new RegisterUserCommand
			{
				UserName = request.UserName,
				Email = request.Email,
				Password = request.Password,
				ConfirmPassword = request.ConfirmPassword,
				Gender = request.Gender,
				Age = request.Age
			};

			var result = await mediator.Send(command);

			if(result.IsSuccess)
			{
                logger.LogInformation("User registration successful for email: {Email}", request.Email);
                return Ok(result);
            }
            logger.LogWarning("User registration failed for email: {Email}", request.Email);
            return BadRequest(result);
        }
        catch (FluentValidation.ValidationException validationEx)
        {
            var validationErrors = validationEx.Errors.Select(e => e.ErrorMessage).ToArray();
            logger.LogWarning("Validation failed for registration: {Errors}", string.Join(", ", validationErrors));
            return BadRequest(AuthResponse.Failed(validationErrors));
        }
        catch (Exception ex)
		{
            logger.LogError(ex, "Error in Register endpoint for email: {Email}", request.Email);
            return BadRequest(AuthResponse.Failed("An unexpected error occurred during registration."));
        }
    }

	[HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
		try
		{
			var query = new LoginUserQuery
			{
				Email = request.Email,
				Password = request.Password,
				RememberMe = request.RememberMe
			};
			var result = await mediator.Send(query);

			if (result.IsSuccess)
			{
                logger.LogInformation("User login successful for email: {Email}", request.Email);
                return Ok(result);
            }

            logger.LogWarning("User login failed for email: {Email}", request.Email);
            return BadRequest(result);
        }
		catch (Exception ex)
		{
            logger.LogError(ex, "Error in Login endpoint for email: {Email}", request.Email);
            return BadRequest(AuthResponse.Failed("An unexpected error occurred during login."));
        }
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userInfo = new UserDto
            {
                Id = int.Parse(userId),
                UserName = User.Identity?.Name ?? string.Empty,
                Email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                Gender = int.TryParse(User.FindFirst(ClaimTypes.Gender)?.Value, out var genderValue)
                    ? (Gender)genderValue
                    : Gender.NotSpecified,
                Age = int.TryParse(User.FindFirst("age")?.Value, out var ageValue) && ageValue > 0
                    ? ageValue
                    : null,
                IsActive = bool.TryParse(User.FindFirst("isActive")?.Value, out var isActive) && isActive,
                Roles = [..User.FindAll(ClaimTypes.Role).Select(r => r.Value)]
            };
            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetCurrentUser endpoint");
            return BadRequest("An error occurred while retrieving user information.");
        }
    }
}
