
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.User;

namespace CollabParty.Application.Common.Dtos.Auth;
public class LoginResponseDto
{
    public UserDtoResponse User { get; set; }
    public TokenResponseDto TokensResponse { get; set; }

}