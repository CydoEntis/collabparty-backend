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

            var partyMembers = await _unitOfWork.PartyMember.GetAllAsync(p => p.PartyId == foundQuest.PartyId, includeProperties: "User.UnlockedAvatars.Avatar");
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