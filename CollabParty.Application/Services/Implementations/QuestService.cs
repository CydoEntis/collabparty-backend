using AutoMapper;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

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
}