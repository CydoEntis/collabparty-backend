using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result> Login(LoginRequestDto requestDto);
    
    // Task<Result<TokenResponseDto>> Login(LoginRequestDto requestDto);
    // Task<Result<LoginResponseDto>> Register(RegisterRequestDto dto);
    // Task<Result> Logout(TokenResponseDto responseDto);
    // Task<Result<TokenResponseDto>> RefreshTokens(TokenResponseDto responseDto);

    Task<Result> Logout();
    
    Task<Result<TokenResponseDto>> RefreshTokens();
    
    Task<Result> ResetPasswordAsync(ResetPasswordRequestDto requestDto);
    Task<Result> SendForgotPasswordEmail(ForgotPasswordRequestDto requestDto);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequestDto requestDto);
}