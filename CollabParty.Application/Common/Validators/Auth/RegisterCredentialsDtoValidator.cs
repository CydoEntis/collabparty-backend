using CollabParty.Application.Common.Dtos;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.Auth;

public class RegisterCredentialsDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterCredentialsDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.").MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.").MaximumLength(20)
            .WithMessage("Username may not exceed 20 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
    }
}