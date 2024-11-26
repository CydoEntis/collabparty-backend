
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.User;

namespace CollabParty.Application.Common.Dtos.Auth;
public class LoginDto
{
    public UserDto User { get; set; }
    public TokenDto Tokens { get; set; }

}