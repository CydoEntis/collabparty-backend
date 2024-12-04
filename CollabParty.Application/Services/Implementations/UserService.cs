using AutoMapper;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Interfaces;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateUserResponseDto>> UpdateUserDetails(string userId, UpdateUserRequestDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<UpdateUserResponseDto>.Failure("User ID is required");

            var foundUser = await _unitOfWork.User.GetAsync(u => u.Id == userId);

            if (foundUser == null)
                return Result<UpdateUserResponseDto>.Failure("User does not exist");

            var emailTaken = await _unitOfWork.User.GetAsync(u => u.Email == dto.Email && u.Id != userId);
            if (emailTaken != null)
                return Result<UpdateUserResponseDto>.Failure("Email is taken");

            var usernameTaken =
                await _unitOfWork.User.GetAsync(u => u.UserName == dto.Username && u.Id != userId);
            if (usernameTaken != null)
                return Result<UpdateUserResponseDto>.Failure("Username is taken");

            foundUser.Email = dto.Email;
            foundUser.UserName = dto.Username;

            await _unitOfWork.User.UpdateAsync(foundUser);

            var updatedUserDto = _mapper.Map<UpdateUserResponseDto>(foundUser);

            return Result<UpdateUserResponseDto>.Success(updatedUserDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user details");
            return Result<UpdateUserResponseDto>.Failure("An error occurred while updating the user");
        }
    }
}