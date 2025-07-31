using Blog.Application.DTOs.Authentication;
using MediatR;

namespace Blog.Application.Features.Authentication.Queries.LoginUser;

public class LoginUserQuery : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}
