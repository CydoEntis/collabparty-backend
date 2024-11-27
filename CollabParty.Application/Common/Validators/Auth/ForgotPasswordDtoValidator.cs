using CollabParty.Application.Common.Dtos.Auth;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.Auth;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress()
            .WithMessage("Invalid email format.");
    }
}