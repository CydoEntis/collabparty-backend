using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Implementations;

public class QuestService : IQuestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestService> _logger;
    private readonly IMapper _mapper;
    private readonly IQuestStepService _questStepService;
    private readonly IQuestAssignmentService _questAssignmentService;
    private readonly IUserService _userService;

    public QuestService(IUnitOfWork unitOfWork, ILogger<QuestService> logger, IMapper mapper,
        IQuestStepService questStepService, IQuestAssignmentService questAssignmentService, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _questStepService = questStepService;
        _questAssignmentService = questAssignmentService;
        _userService = userService;
    }

    public async Task<Result> CreateQuest(string userId, CreateQuestRequestDto dto)
    {
        try
        {
            var newQuest = _mapper.Map<Quest>(dto);
            newQuest.CreatedById = userId;
            newQuest.ExpReward = CalculateQuestExpReward(dto.PriorityLevel);
            newQuest.GoldReward = CalculateQuestGoldReward(dto.PriorityLevel);
            var createdQuest = await _unitOfWork.Quest.CreateAsync(newQuest);
            await _questStepService.CreateQuestSteps(createdQuest.Id, dto.Steps);
            await _questAssignmentService.AssignPartyMembersToQuest(createdQuest.Id, dto.PartyMembers);

            return Result.Success("Quest created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create quest.");
            return Result.Failure("An error occurred while creating the party.");
        }
    }

    public async Task<Result<PaginatedResult<QuestResponseDto>>> GetQuests(string userId, int partyId,
        QueryParamsDto dto)
    {
        try
        {
            var queryParams = new QueryParams<Quest>
            {
                Search = dto.Search,
                OrderBy = dto.OrderBy,
                SortBy = dto.SortBy,
                DateFilter = dto.DateFilter,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                IncludeProperties =
                    "QuestAssignments.User.UnlockedAvatars.Avatar,QuestSteps",
                Filter = q =>
                    q.PartyId == partyId && !q.IsCompleted && q.QuestAssignments.Any(qa => qa.UserId == userId),
            };

            var paginatedResult = await _unitOfWork.Quest.GetPaginatedAsync(queryParams);


            var questDtos = _mapper.Map<List<QuestResponseDto>>(paginatedResult.Items);

            var result = new PaginatedResult<QuestResponseDto>(questDtos, paginatedResult.TotalItems,
                paginatedResult.CurrentPage, queryParams.PageSize);

            return Result<PaginatedResult<QuestResponseDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user parties.");
            return Result<PaginatedResult<QuestResponseDto>>.Failure("An error occurred while fetching parties.");
        }
    }


    public async Task<Result<QuestDetailResponseDto>> GetQuest(int questId)
    {
        try
        {
            var foundQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties:
                "QuestAssignments.User.UnlockedAvatars.Avatar,QuestSteps,QuestComments,QuestFiles");


            if (foundQuest == null)
                return Result<QuestDetailResponseDto>.Failure($"No party with the {questId} exists");

            var partyMembers = await _unitOfWork.PartyMember.GetAllAsync(p => p.PartyId == foundQuest.PartyId,
                includeProperties: "User.UnlockedAvatars.Avatar");
            var partyMembersDto = _mapper.Map<List<PartyMemberResponseDto>>(partyMembers);

            var questDetailDto = _mapper.Map<QuestDetailResponseDto>(foundQuest);
            questDetailDto.PartyMembers = partyMembersDto;
            return Result<QuestDetailResponseDto>.Success(questDetailDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party with id");
            return Result<QuestDetailResponseDto>.Failure("An error occurred while fetching party.");
        }
    }

    public async Task<Result<int>> CompleteQuest(string userId, int questId)
    {
        try
        {
            var foundQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties:
                "Party.PartyMembers.User.UnlockedAvatars.Avatar,QuestSteps,QuestComments,QuestFiles");

            if (foundQuest == null)
                return Result<int>.Failure($"No party with the {questId} exists");

            foundQuest.IsCompleted = true;
            foundQuest.CompletedById = userId;

            foreach (var questStep in foundQuest.QuestSteps)
            {
                questStep.IsCompleted = true;
                questStep.CompletedAt = DateTime.UtcNow;
            }

            await _userService.AddExperience(userId, foundQuest.ExpReward);
            await _userService.AddGold(userId, foundQuest.GoldReward);

            await _unitOfWork.Quest.UpdateAsync(foundQuest);

            return Result<int>.Success(foundQuest.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party with id");
            return Result<int>.Failure("An error occurred while completing quest.");
        }
    }

    public async Task<Result<int>> UpdateQuest(string userId, int questId, UpdateQuestRequestDto dto)
    {
        try
        {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId);

            if (user.Role is not (UserRole.Leader or UserRole.Captain))
            {
                return Result<int>.Failure("You do not have permission to update quests.");
            }


            var existingQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties: "QuestSteps,QuestAssignments"
            );

            if (existingQuest == null)
            {
                return Result<int>.Failure($"Quest with ID {questId} not found.");
            }

            // Map updated properties
            existingQuest.Name = dto.Name;
            existingQuest.Description = dto.Description;
            existingQuest.PriorityLevel = dto.PriorityLevel;
            existingQuest.DueDate = dto.DueDate;

            // Update rewards based on priority level
            existingQuest.ExpReward = CalculateQuestExpReward(dto.PriorityLevel);
            existingQuest.GoldReward = CalculateQuestGoldReward(dto.PriorityLevel);

            existingQuest.UpdatedAt = DateTime.UtcNow;

            // Update quest steps
            if (dto.Steps != null && dto.Steps.Any())
            {
                var stepUpdateResult = await _questStepService.UpdateQuestSteps(existingQuest.Id, dto.Steps);
                if (!stepUpdateResult.IsSuccess)
                {
                    return Result<int>.Failure(stepUpdateResult.Message);
                }
            }

            // Update quest assignments
            if (dto.AssignedPartyMembers != null)
            {
                var assignmentUpdateResult =
                    await _questAssignmentService.UpdateQuestAssignments(existingQuest.Id, dto.AssignedPartyMembers);
                if (!assignmentUpdateResult.IsSuccess)
                {
                    return Result<int>.Failure(assignmentUpdateResult.Message);
                }
            }

            await _unitOfWork.Quest.UpdateAsync(existingQuest);

            return Result<int>.Success(existingQuest.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update quest.");
            return Result<int>.Failure("An error occurred while updating the quest.");
        }
    }


    public async Task<Result<int>> DeleteQuest(string userId, int questId)
    {
        try
        {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId);

            if (user.Role is not (UserRole.Leader or UserRole.Captain))
            {
                return Result<int>.Failure("You do not have permission to update quests.");
            }


            var existingQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties: "QuestSteps,QuestAssignments"
            );

            if (existingQuest == null)
            {
                return Result<int>.Failure($"Quest with ID {questId} not found.");
            }

            // Remove quest steps
            foreach (var step in existingQuest.QuestSteps.ToList())
            {
                await _unitOfWork.QuestStep.RemoveAsync(step);
            }

            // Remove quest assignments
            foreach (var assignment in existingQuest.QuestAssignments.ToList())
            {
                await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
            }

            // Remove the quest itself
            await _unitOfWork.Quest.RemoveAsync(existingQuest);

            return Result<int>.Success(existingQuest.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete quest.");
            return Result<int>.Failure("An error occurred while deleting the quest.");
        }
    }


    private int CalculateQuestExpReward(PriorityLevelOption priorityLevel)
    {
        return priorityLevel switch
        {
            PriorityLevelOption.Low => 50,
            PriorityLevelOption.Medium => 100,
            PriorityLevelOption.High => 200,
            PriorityLevelOption.Critical => 500,
            _ => 0
        };
    }

    private int CalculateQuestGoldReward(PriorityLevelOption priorityLevel)
    {
        return priorityLevel switch
        {
            PriorityLevelOption.Low => 10,
            PriorityLevelOption.Medium => 20,
            PriorityLevelOption.High => 40,
            PriorityLevelOption.Critical => 100,
            _ => 0
        };
    }
}