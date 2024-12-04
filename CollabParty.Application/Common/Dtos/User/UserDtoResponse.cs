using CollabParty.Application.Common.Dtos.Avatar;

namespace CollabParty.Application.Common.Dtos.User;
public class UserDtoResponse
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public int Gold { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentExp { get; set; }
    public int ExpToNextLevel { get; set; }
    public AvatarResponseDto AvatarResponse { get; set; }
}