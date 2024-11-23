using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginDto>> Login(LoginCredentialsDto loginCredentialsDto);
    Task<Result<LoginDto>> Register(RegisterCredentialsDto registerDto);
}