using System.ComponentModel.DataAnnotations;

namespace CollabParty.Application.Common.Dtos;

public class LoginRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}