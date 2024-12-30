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
using CollabParty.Application.Common.Utility;

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


    public async Task<ResponseDto> Register(RegisterRequestDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (!EntityUtility.EntityIsNull(existingUser))
        {
            if (existingUser.Email == dto.Email)
                throw new ValidationException(ErrorFields.Email, ErrorMessages.EmailInUse);

            if (existingUser.UserName == dto.Username)
                throw new ValidationException(ErrorFields.UserName, ErrorMessages.UsernameInUse);
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
            if (creationResult.Errors.Any(e => e.Code == "DuplicateUserName"))
                throw new AlreadyExistsException("username", ErrorMessages.UsernameInUse);
            else
                throw new ResourceCreationException(ErrorMessages.RegistrationFailed);
        }


        await _unlockedAvatarService.UnlockStarterAvatars(user);
        await _unlockedAvatarService.SetNewUserAvatar(user.Id, dto.AvatarId);


        await Login(new LoginRequestDto() { Email = dto.Email, Password = dto.Password });

        return new ResponseDto() { Message = SuccessMessages.RegistrationSuccessful };
    }

    public async Task<ResponseDto> Login(LoginRequestDto requestDto)
    {
        var user = await _unitOfWork.User.GetAsync(u => u.Email == requestDto.Email);
        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException(ErrorMessages.UserNotFound);

        if (!await _userManager.CheckPasswordAsync(user, requestDto.Password))
            throw new ValidationException(ErrorFields.Email, ErrorMessages.InvalidCredentials);


        var sessionId = $"SESS{Guid.NewGuid()}";
        var refreshToken = _tokenService.CreateRefreshToken();
        _tokenService.CreateCsrfToken();

        await _sessionService.CreateSession(user.Id, sessionId, refreshToken);
        _tokenService.CreateAccessToken(user.Id, sessionId);

        return new ResponseDto() { Message = SuccessMessages.LoginSuccessful };
    }

    public async Task<ResponseDto> Logout()
    {
        var refreshToken = _cookieService.Get(CookieNames.RefreshToken);
        if (string.IsNullOrEmpty(refreshToken))
            throw new NotFoundException(ErrorMessages.TokenNotFound);


        var session = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);
        if (EntityUtility.EntityIsNull(session))
            throw new NotFoundException(ErrorMessages.SessionNotFound);

        await _sessionService.InvalidateSession(session);
        _cookieService.Delete(CookieNames.RefreshToken);
        _cookieService.Delete(CookieNames.AccessToken);
        return new ResponseDto() { Message = SuccessMessages.LogoutSuccessful };
    }

    public async Task<ResponseDto> RefreshTokens()
    {
        var refreshToken = _cookieService.Get("QB-REFRESH-TOKEN");
        if (string.IsNullOrEmpty(refreshToken))
            throw new NotFoundException(ErrorMessages.TokenNotFound);


        var session = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);
        if (EntityUtility.EntityIsNull(session))
            throw new NotFoundException(ErrorMessages.SessionNotFound);

        if (!_tokenService.IsRefreshTokenValid(session, refreshToken))
            throw new InvalidTokenException(ErrorMessages.TokenExpired);

        var user = await _unitOfWork.User.GetAsync(u => u.Id == session.UserId);
        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException(ErrorMessages.UserNotFound);


        var accessToken = _tokenService.CreateAccessToken(user.Id, session.SessionId);
        var refreshTokenModel = _tokenService.CreateRefreshToken();
        session.RefreshToken = refreshTokenModel.Token;
        session.RefreshTokenExpiry = refreshTokenModel.Expiry;

        await _unitOfWork.Session.UpdateAsync(session);
        return new ResponseDto() { Message = SuccessMessages.TokenRefreshSuccessful };
    }

    public async Task<ResponseDto> SendForgotPasswordEmail(ForgotPasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);

        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException(ErrorMessages.UserNotFound);

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

        return new ResponseDto() { Message = SuccessMessages.PasswordResetEmailSent };
    }

    public async Task<ResponseDto> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);
        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException(ErrorMessages.UserNotFound);

        var decodedToken = Uri.UnescapeDataString(requestDto.Token);

        var tokenIsValid = await _userManager.VerifyUserTokenAsync(
            user,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            "ResetPassword",
            decodedToken
        );

        if (!tokenIsValid)
            throw new InvalidTokenException(ErrorMessages.TokenInvalid);

        var currentPasswordHash = user.PasswordHash;
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var passwordVerificationResult =
            passwordHasher.VerifyHashedPassword(user, currentPasswordHash, requestDto.NewPassword);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
            throw new AlreadyExistsException(ErrorFields.NewPassword, ErrorMessages.OldPassword);

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, decodedToken, requestDto.NewPassword);
        if (!resetPasswordResult.Succeeded)
            throw new OperationException(ErrorTitles.PasswordResetException,
                ErrorMessages.PasswordResetFailed);

        return new ResponseDto() { Message = SuccessMessages.PasswordResetSuccess };
    }


    public async Task<ResponseDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto requestDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException(ErrorMessages.UserNotFound);

        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, requestDto.CurrentPassword);
        if (!isCurrentPasswordValid)
            throw new ValidationException(ErrorFields.CurrentPassword, ErrorMessages.CurrentPasswordMismatch);

        var updateResult =
            await _userManager.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword);
        if (!updateResult.Succeeded)
            throw new ResourceModificationException(ErrorMessages.ChangePasswordFailed);

        return new ResponseDto() { Message = SuccessMessages.PasswordChangedSuccessfully };
    }
}