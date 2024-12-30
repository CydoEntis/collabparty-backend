using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.General;
using CollabParty.Application.Common.Errors;

namespace CollabParty.Application.Services.Interfaces;

public interface IAuthService
{
    Task<ResponseDto> Register(RegisterRequestDto dto);
    Task<ResponseDto> Login(LoginRequestDto requestDto);
    Task<ResponseDto> Logout();
    Task<ResponseDto> RefreshTokens();
    Task<ResponseDto> ResetPasswordAsync(ResetPasswordRequestDto requestDto);
    Task<ResponseDto> SendForgotPasswordEmail(ForgotPasswordRequestDto requestDto);
    Task<ResponseDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto requestDto);
}