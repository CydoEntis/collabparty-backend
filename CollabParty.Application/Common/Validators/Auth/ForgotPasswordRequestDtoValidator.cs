using CollabParty.Application.Common.Dtos.Auth;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.Auth;

public class ForgotPasswordRequestDtoValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress()
            .WithMessage("Invalid email format.");
    }
}