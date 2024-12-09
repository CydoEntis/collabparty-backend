using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result> Register(RegisterRequestDto dto);
    Task<Result> Login(LoginRequestDto requestDto);

    Task<Result> Logout();

    Task<Result> RefreshTokens();
    
    Task<Result> ResetPasswordAsync(ResetPasswordRequestDto requestDto);
    Task<Result> SendForgotPasswordEmail(ForgotPasswordRequestDto requestDto);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequestDto requestDto);
}