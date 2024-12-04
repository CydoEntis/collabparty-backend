using CollabParty.Application.Common.Dtos;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.Auth;

public class LoginCredentialsRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginCredentialsRequestDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
    }
}