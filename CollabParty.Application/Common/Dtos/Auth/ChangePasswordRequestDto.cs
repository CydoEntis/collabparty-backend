namespace CollabParty.Application.Common.Dtos.Auth;

public class ChangePasswordRequestDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}