using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.General;
using CollabParty.Application.Common.Errors;

namespace CollabParty.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<ResponseDto>> Register(RegisterRequestDto dto);
    Task<Result<ResponseDto>> Login(LoginRequestDto requestDto);
    Task<string> Logout();
    Task<string> RefreshTokens();
    Task<string> ResetPasswordAsync(ResetPasswordRequestDto requestDto);
    Task<string> SendForgotPasswordEmail(ForgotPasswordRequestDto requestDto);
    Task<string> ChangePasswordAsync(string userId, ChangePasswordRequestDto requestDto);
}