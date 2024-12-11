using System.Diagnostics;
using System.Text;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using AutoMapper;
using CollabParty.Application.Common.Constants;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Mappings;
using CollabParty.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

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
        ISessionService sessionService, ICookieService cookieService, IEmailTemplateService emailTemplateService, IUnlockedAvatarService unlockedAvatarService)
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


    public async Task<Result> Login(LoginRequestDto requestDto)
    {
        var user = await _unitOfWork.User.GetAsync(u => u.Email == requestDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, requestDto.Password))
            return Result.Failure("email", new[] { "Invalid username or password" });

        var sessionId = $"SESS{Guid.NewGuid()}";
        var refreshToken = _tokenService.CreateRefreshToken();
        _tokenService.CreateCsrfToken();

        await _sessionService.CreateSession(user.Id, sessionId, refreshToken);

        _tokenService.CreateAccessToken(user.Id, sessionId);
        return Result.Success("Login successful");
    }


    public async Task<Result> Register(RegisterRequestDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return Result.Failure("email", new[] { "A user with this email already exists" });
        }

        var existingUserByUsername = await _userManager.FindByNameAsync(dto.Username);
        if (existingUserByUsername != null)
        {
            return Result.Failure("username", new[] { "A user with this username already exists" });
        }

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
        {
            var errors = creationResult.Errors
                .Select(e => new ValidationError("user", new[] { e.Description }))
                .ToList();
            return Result.Failure(errors);
        }

        await _unlockedAvatarService.UnlockStarterAvatars(user);
        await _unlockedAvatarService.SetNewUserAvatar(user.Id, dto.AvatarId);

        var loginCredentialsDto = _mapper.Map<LoginRequestDto>(dto);

        var loginResult = await Login(loginCredentialsDto);
        if (!loginResult.IsSuccess)
        {
            return Result.Failure("Failed to register");
        }

        return Result.Success("Registration successful, you are now logged in");
    }

    public async Task<Result> Logout()
    {
        var refreshToken = _cookieService.Get("QB-REFRESH-TOKEN");
        if (string.IsNullOrEmpty(refreshToken))
            return Result.Failure("refreshToken", new[] { "Refresh token not found." });

        var session = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);
        if (session == null)
            return Result.Failure("session", new[] { "Session not found or invalidated." });

        await _sessionService.InvalidateSession(session);

        _cookieService.Delete(CookieNames.RefreshToken);
        _cookieService.Delete(CookieNames.AccessToken);
        _cookieService.Delete(CookieNames.CsrfToken);

        return Result.Success("Logged out successfully.");
    }


    public async Task<Result> RefreshTokens()
    {
        var refreshToken = _cookieService.Get("QB-REFRESH-TOKEN");
        if (string.IsNullOrEmpty(refreshToken))
            return Result.Failure("refreshToken", new[] { "Refresh token not found." });

        var session = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);
        if (session == null || !_tokenService.IsRefreshTokenValid(session, refreshToken))
            return Result.Failure("refreshToken", new[] { "Invalid or expired refresh token." });

        var user = await _unitOfWork.User.GetAsync(u => u.Id == session.UserId);
        if (user == null)
            return Result.Failure("user", new[] { "User not found." });

        var accessToken = _tokenService.CreateAccessToken(user.Id, session.SessionId);
        var refreshTokenModel = _tokenService.CreateRefreshToken();

        session.RefreshToken = refreshTokenModel.Token;
        session.RefreshTokenExpiry = refreshTokenModel.Expiry;
        await _unitOfWork.Session.UpdateAsync(session);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);
        if (user == null)
        {
            return Result.Failure("email", new[] { "No user found with that email address." });
        }

        var decodedToken = Uri.UnescapeDataString(requestDto.Token);

        var tokenIsValid = await _userManager.VerifyUserTokenAsync(
            user,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            "ResetPassword",
            decodedToken
        );

        if (!tokenIsValid)
        {
            return Result.Failure("token", new[] { "Token is no longer valid." });
        }

        var currentPasswordHash = user.PasswordHash;
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var passwordVerificationResult =
            passwordHasher.VerifyHashedPassword(user, currentPasswordHash, requestDto.NewPassword);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
        {
            return Result.Failure("newPassword", new[] { "New password cannot be the same as a previous password." });
        }

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, decodedToken, requestDto.NewPassword);
        if (!resetPasswordResult.Succeeded)
        {
            var errors = resetPasswordResult.Errors
                .Select(e => new ValidationError("newPassword", new[] { e.Description }))
                .ToList();
            return Result.Failure(errors);
        }

        return Result.Success("Password has been successfully reset.");
    }


    public async Task<Result> SendForgotPasswordEmail(ForgotPasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);

        if (user != null)
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            Debug.WriteLine(resetToken);

            var encodedToken = Uri.EscapeDataString(resetToken);

            var resetUrl = $"http://localhost:5173/reset-password?token={encodedToken}";

            var placeholders = new Dictionary<string, string>
            {
                { "Recipient's Email", requestDto.Email },
                { "Reset Link", resetUrl }
            };

            var emailBody = _emailTemplateService.GetEmailTemplate("ForgotPasswordTemplate", placeholders);

            await _emailService.SendEmailAsync(requestDto.Email, "Password Reset Request", emailBody);
        }

        return Result.Success("If an account with that email exists, a password reset link will be sent.");
    }


    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("user", new[] { "User not found" });
        }

        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, requestDto.CurrentPassword);
        if (!isCurrentPasswordValid)
        {
            return Result.Failure("currentPassword", new[] { "Current password is incorrect" });
        }

        var updateResult =
            await _userManager.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword);
        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors
                .Select(e => new ValidationError("password", new[] { e.Description }))
                .ToList();
            return Result.Failure(errors);
        }

        return Result.Success("Password changed successfully");
    }
}