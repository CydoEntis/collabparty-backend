namespace CollabParty.Application.Common.Dtos.Auth;

public class ChangePasswordDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}