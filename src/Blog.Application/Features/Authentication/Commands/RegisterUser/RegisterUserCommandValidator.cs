using FluentValidation;

namespace Blog.Application.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please provide a valid email address.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Password and confirmation password do not match.");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(13).WithMessage("Age must be at least 13 years.")
            .LessThanOrEqualTo(120).WithMessage("Age cannot exceed 120 years.")
            .When(x => x.Age.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Please select a valid gender.");
    }
}
