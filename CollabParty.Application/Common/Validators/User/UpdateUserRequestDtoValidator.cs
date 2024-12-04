using CollabParty.Application.Common.Dtos.User;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.User;

public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(20)
            .WithMessage("Username may not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));
    }
}