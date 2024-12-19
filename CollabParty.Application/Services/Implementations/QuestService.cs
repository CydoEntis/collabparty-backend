using AutoMapper;
using CollabParty.Application.Common.Dtos;
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

    public QuestService(IUnitOfWork unitOfWork, ILogger<QuestService> logger, IMapper mapper,
        IQuestStepService questStepService, IQuestAssignmentService questAssignmentService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _questStepService = questStepService;
        _questAssignmentService = questAssignmentService;
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
                    "Party.PartyMembers.User.UnlockedAvatars.Avatar,QuestSteps",
                Filter = q =>
                    q.Party.PartyMembers.Any(qm => qm.UserId == userId) && q.PartyId == partyId,
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