namespace CollabParty.Application.Common.Dtos.Auth;

public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}