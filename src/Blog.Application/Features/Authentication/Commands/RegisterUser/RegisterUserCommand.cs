using Blog.Application.DTOs.Authentication;
using Blog.Domain.Enums;
using MediatR;

namespace Blog.Application.Features.Authentication.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<AuthResponse>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.NotSpecified;
        public int? Age { get; set; }
    }
}
