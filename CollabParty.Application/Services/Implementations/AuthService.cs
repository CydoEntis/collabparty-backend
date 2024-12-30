using System.Diagnostics;
using System.Text;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using AutoMapper;
using CollabParty.Application.Common.Constants;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.General;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Mappings;
using CollabParty.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly ISessionService _sessionService;
    private readonly ICookieService _cookieService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IUnlockedAvatarService _unlockedAvatarService;

    public AuthService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IMapper mapper,
        ITokenService tokenService,
        ISessionService sessionService, ICookieService cookieService, IEmailTemplateService emailTemplateService,
        IUnlockedAvatarService unlockedAvatarService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
        _mapper = mapper;
        _tokenService = tokenService;
        _sessionService = sessionService;
        _cookieService = cookieService;
        _emailTemplateService = emailTemplateService;
        _unlockedAvatarService = unlockedAvatarService;
    }


    public async Task<Result<ResponseDto>> Register(RegisterRequestDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new ValidationException("email", "Email already in use");

        var existingUserByUsername = await _userManager.FindByNameAsync(dto.Username);
        if (existingUserByUsername != null)
            throw new ValidationException("username", "Username already in use");


        ApplicationUser user = new()
        {
            UserName = dto.Username,
            Email = dto.Email,
            NormalizedEmail = dto.Email.ToUpper(),
            NormalizedUserName = dto.Username.ToUpper(),
            CurrentExp = 0,
            CurrentLevel = 1,
            ExpToNextLevel = 100,
            CreatedAt = DateTime.UtcNow
        };

        var creationResult = await _userManager.CreateAsync(user, dto.Password);
        if (!creationResult.Succeeded)
            throw new CreationException("Registration failed");

        await _unlockedAvatarService.UnlockStarterAvatars(user);
        await _unlockedAvatarService.SetNewUserAvatar(user.Id, dto.AvatarId);

        var loginCredentialsDto = _mapper.Map<LoginRequestDto>(dto);

        var loginResult = await Login(loginCredentialsDto);

        return Result<ResponseDto>.Success(new ResponseDto() { Message = "Registration Successful" });
    }

    public async Task<Result<ResponseDto>> Login(LoginRequestDto requestDto)
    {
        var user = await _unitOfWork.User.GetAsync(u => u.Email == requestDto.Email);
        if (user == null)
            throw new NotFoundException("User not found");

        if (!await _userManager.CheckPasswordAsync(user, requestDto.Password))
            throw new ValidationException("username", "Username or password is incorrect");


        var sessionId = $"SESS{Guid.NewGuid()}";
        var refreshToken = _tokenService.CreateRefreshToken();
        _tokenService.CreateCsrfToken();

        await _sessionService.CreateSession(user.Id, sessionId, refreshToken);
        _tokenService.CreateAccessToken(user.Id, sessionId);

        return Result<ResponseDto>.Success(new ResponseDto() { Message = "Login Successful" });
    }

    public async Task<string> Logout()
    {
        var refreshToken = _cookieService.Get("QB-REFRESH-TOKEN");
        if (string.IsNullOrEmpty(refreshToken))
            throw new NotFoundException("Refresh token not found");


        var session = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);
        if (session == null)
            throw new NotFoundException("Session not found");

        await _sessionService.InvalidateSession(session);
        _cookieService.Delete(CookieNames.RefreshToken);
        _cookieService.Delete(CookieNames.AccessToken);
        return "Logout successful";
    }

    public async Task<string> RefreshTokens()
    {
        var refreshToken = _cookieService.Get("QB-REFRESH-TOKEN");
        if (string.IsNullOrEmpty(refreshToken))
            throw new NotFoundException("Refresh token not found");


        var session = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);
        if (session == null)
            throw new NotFoundException("Session not found");

        if (_tokenService.IsRefreshTokenValid(session, refreshToken))
            throw new InvalidTokenException("Refresh token expired");

        var user = await _unitOfWork.User.GetAsync(u => u.Id == session.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        var accessToken = _tokenService.CreateAccessToken(user.Id, session.SessionId);
        var refreshTokenModel = _tokenService.CreateRefreshToken();
        session.RefreshToken = refreshTokenModel.Token;
        session.RefreshTokenExpiry = refreshTokenModel.Expiry;

        await _unitOfWork.Session.UpdateAsync(session);
        return "Tokens refreshed successfully";
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);
        if (user == null)
            throw new NotFoundException("User not found");

        var decodedToken = Uri.UnescapeDataString(requestDto.Token);

        var tokenIsValid = await _userManager.VerifyUserTokenAsync(
            user,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            "ResetPassword",
            decodedToken
        );

        if (!tokenIsValid)
            throw new InvalidTokenException("Password reset token is invalid token");

        var currentPasswordHash = user.PasswordHash;
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var passwordVerificationResult =
            passwordHasher.VerifyHashedPassword(user, currentPasswordHash, requestDto.NewPassword);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
            throw new DuplicateException("newPassword", "Cannot use a previous password.");

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, decodedToken, requestDto.NewPassword);
        if (!resetPasswordResult.Succeeded)
            throw new ServiceException(StatusCodes.Status400BadRequest, "Reset Password Error",
                "Password reset failed");

        return "Password has been successfully reset.";
    }

    public async Task<string> SendForgotPasswordEmail(ForgotPasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);

        if (user == null)
            throw new NotFoundException("User not found");

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(resetToken);
        var resetUrl = $"http://localhost:5173/reset-password?token={encodedToken}";
        var placeholders = new Dictionary<string, string>
        {
            { "Recipient's Email", requestDto.Email },
            { "Reset Link", resetUrl }
        };

        var emailBody = _emailTemplateService.GetEmailTemplate("ForgotPasswordTemplate", placeholders);

        await _emailService.SendEmailAsync(requestDto.Email, "Password Reset Request", emailBody);

        return "If an account with that email exists, a password reset link will be sent.";
    }

    public async Task<string> ChangePasswordAsync(string userId, ChangePasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, requestDto.CurrentPassword);
        if (!isCurrentPasswordValid)
            throw new ValidationException("currentPassword", "Current password is incorrect.");

        var updateResult =
            await _userManager.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword);
        if (!updateResult.Succeeded)
            throw new ServiceException(StatusCodes.Status400BadRequest, "Change Password Error",
                "Changing password failed");

        return "Password changed successfully";
    }
}