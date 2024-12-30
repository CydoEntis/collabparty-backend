using CollabParty.Application.Common.Dtos;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.Auth;

public class LoginCredentialsRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginCredentialsRequestDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress()
            .WithMessage("Invalid email format.").WithName("emailAddress");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.").WithName("password");
    }
}