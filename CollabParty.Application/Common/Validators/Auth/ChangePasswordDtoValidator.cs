using CollabParty.Application.Common.Dtos.Auth;
using FluentValidation;

namespace CollabParty.Application.Common.Validators.Auth;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Current password is requred");
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
    }
}