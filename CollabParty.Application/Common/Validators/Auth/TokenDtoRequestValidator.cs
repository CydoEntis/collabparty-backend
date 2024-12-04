using FluentValidation;
using CollabParty.Application.Common.Dtos;

namespace CollabParty.Application.Common.Validators.Auth;

public class TokenDtoRequestValidator : AbstractValidator<TokenResponseDto>
{
    public TokenDtoRequestValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access Token is required.");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh Token is required.");
    }
}