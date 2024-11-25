using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public static class AuthMapper
{
    public static TokenDto ToTokenDto(Tokens tokens)
    {
        return new TokenDto()
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }


    public static LoginDto ToLoginDto(ApplicationUser user, Tokens tokens)
    {
        return new LoginDto
        {
            User = UserMapper.ToUserDto(user),
            Tokens = ToTokenDto(tokens)
        };
    }

    public static LoginCredentialsDto ToLoginCredentialsDto(RegisterCredentialsDto dto)
    {
        return new LoginCredentialsDto
        {
            Email = dto.Email,
            Password = dto.Password
        };
    }
}