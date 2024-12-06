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
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Mappings;
using CollabParty.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CollabParty.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UnlockedAvatarService _unlockedAvatarService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IMapper _mapper;
    private readonly string _jwtSecret;
    private readonly string _jwtAudience;
    private readonly string _jwtIssuer;

    public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
        IConfiguration configuration, IEmailService emailService, IEmailTemplateService emailTemplateService,
        IMapper mapper, UnlockedAvatarService unlockedAvatarService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _mapper = mapper;
        _unlockedAvatarService = unlockedAvatarService;
        _jwtSecret = configuration["JwtSecret"];
        _jwtAudience = configuration["JwtAudience"];
        _jwtIssuer = configuration["JwtIssuer"];
    }

    public async Task<Result<LoginResponseDto>> Login(LoginRequestDto requestDto)
    {
        var user = await _unitOfWork.User.GetAsync(
            u => u.Email == requestDto.Email,
            includeProperties: "UserAvatars,UserAvatars.Avatar");

        if (user == null)
            return Result<LoginResponseDto>.Failure("email", new[] { "Invalid username or password" });

        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, requestDto.Password);

        if (!isPasswordValid)
            return Result<LoginResponseDto>.Failure("email", new[] { "Invalid username or password" });

        var sessionId = $"SESS{Guid.NewGuid()}";
        var accessToken = CreateAccessToken(user, sessionId);
        var refreshToken = await CreateRefreshToken(user.Id, sessionId);
        var userAvatar = await _unitOfWork.UnlockedAvatar.GetAsync(
            ua => ua.UserId == user.Id && ua.IsActive,
            includeProperties: "Avatar");


        if (userAvatar == null)
            return Result<LoginResponseDto>.Failure("avatar", new[] { "Active avatar not found for user" });

        var tokenDto = new TokenResponseDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
        var loginDto = _mapper.Map<LoginResponseDto>(user);
        loginDto.Tokens = tokenDto;

        return Result<LoginResponseDto>.Success(loginDto);
    }


    public async Task<Result<LoginResponseDto>> Register(RegisterRequestDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return Result<LoginResponseDto>.Failure("email", new[] { "A user with this email already exists" });
        }

        var existingUserByUsername = await _userManager.FindByNameAsync(dto.Username);
        if (existingUserByUsername != null)
        {
            return Result<LoginResponseDto>.Failure("username", new[] { "A user with this username already exists" });
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
            return Result<LoginResponseDto>.Failure(errors);
        }

        await _unlockedAvatarService.UnlockStarterAvatars(user);
        await _unlockedAvatarService.SetNewUserAvatar(user.Id, dto.AvatarId);

        var loginCredentialsDto = _mapper.Map<LoginRequestDto>(dto);

        var loginResult = await Login(loginCredentialsDto);
        if (!loginResult.IsSuccess)
        {
            return loginResult;
        }

        return loginResult;
    }


    public async Task<Result> Logout(TokenResponseDto responseDto)
    {
        var foundSession = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == responseDto.RefreshToken);

        if (foundSession == null)
        {
            return Result.Failure(new List<ValidationError>
            {
                new ValidationError("refreshToken", new[] { "Session not found or already invalidated." })
            });
        }

        var isRefreshTokenValid = CheckIfRefreshTokenIsValid(responseDto.RefreshToken, foundSession);
        if (!isRefreshTokenValid)
        {
            return Result.Failure(new List<ValidationError>
            {
                new ValidationError("refreshToken", new[] { "Invalid refresh token." })
            });
        }

        var isAccessTokenValid = await
            CheckIfAccessTokenIsValid(responseDto.AccessToken, foundSession.UserId, foundSession.SessionId);
        if (!isAccessTokenValid)
        {
            return Result.Failure(new List<ValidationError>
            {
                new ValidationError("accessToken", new[] { "Invalid access token." })
            });
        }

        await _unitOfWork.Session.InvalidateAllUsersTokens(foundSession.UserId, foundSession.SessionId);

        return Result.Success();
    }


    public async Task<Result<TokenResponseDto>> RefreshTokens(TokenResponseDto responseDto)
    {
        var foundSession = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == responseDto.RefreshToken);
        if (foundSession == null)
            return Result<TokenResponseDto>.Failure("refreshToken", new[] { "Refresh token not found." });

        if (!CheckIfRefreshTokenIsValid(responseDto.RefreshToken, foundSession))
        {
            await InvalidateSession(foundSession);
            return Result<TokenResponseDto>.Failure("refreshToken", new[] { "Invalid refresh token." });
        }

        var isAccessTokenValid =
            await CheckIfAccessTokenIsValid(responseDto.AccessToken, foundSession.UserId, foundSession.SessionId);


        if (foundSession.RefreshTokenExpiry < DateTime.UtcNow && !isAccessTokenValid)
        {
            await InvalidateSession(foundSession);
            return Result<TokenResponseDto>.Failure("refreshToken", new[] { "Refresh token expired." });
        }

        var newRefreshToken = await CreateRefreshToken(foundSession.UserId, foundSession.SessionId);
        await InvalidateSession(foundSession);

        var applicationUser = await _unitOfWork.User.GetAsync(u => u.Id == foundSession.UserId);
        if (applicationUser == null)
            return Result<TokenResponseDto>.Failure("user", new[] { "User not found." });

        var newAccessToken = CreateAccessToken(applicationUser, foundSession.SessionId);

        var tokenDto = new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return Result<TokenResponseDto>.Success(tokenDto);
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

    private string CreateAccessToken(ApplicationUser user, string sessionId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, sessionId),
                }
            ),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials,
            Audience = _jwtAudience,
            Issuer = _jwtIssuer
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }

    private async Task<string> CreateRefreshToken(string userId, string sessionId)
    {
        Session session = new()
        {
            IsValid = true,
            UserId = userId,
            SessionId = sessionId,
            RefreshTokenExpiry = DateTime.UtcNow.AddHours(12),
            RefreshToken = Guid.NewGuid() + "-" + Guid.NewGuid()
        };

        await _unitOfWork.Session.CreateAsync(session);

        return session.RefreshToken;
    }

    private async Task InvalidateSession(Session session)
    {
        session.IsValid = false;
        await _unitOfWork.SaveAsync();
    }

    private async Task<bool> CheckIfAccessTokenIsValid(string accessToken, string expectedUserId,
        string expectedSessionId)
    {
        try
        {
            var tokenHandler = new JsonWebTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenValidationResult = await tokenHandler.ValidateTokenAsync(accessToken, tokenValidationParameters);

            var userId = tokenValidationResult.Claims.FirstOrDefault(c => c.Key == "sub").Value?.ToString();
            var sessionId = tokenValidationResult.Claims.FirstOrDefault(c => c.Key == "jti").Value?.ToString();

            return userId == expectedUserId && sessionId == expectedSessionId;
        }
        catch
        {
            return false;
        }
    }

    private bool CheckIfRefreshTokenIsValid(string refreshToken, Session session)
    {
        return session.RefreshToken == refreshToken && session.RefreshTokenExpiry > DateTime.UtcNow;
    }
}