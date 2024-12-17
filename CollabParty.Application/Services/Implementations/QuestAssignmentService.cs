using AutoMapper;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class QuestAssignmentService : IQuestAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestAssignmentService> _logger;
    private readonly IMapper _mapper;

    public QuestAssignmentService(IUnitOfWork unitOfWork, ILogger<QuestAssignmentService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result> AssignPartyMembersToQuest(int questId, string[] partyMemberIds)
    {
        try
        {
            var partyMembers = partyMemberIds.Select(partyMemberId => new QuestAssignment()
            {
                QuestId = questId,
                UserId = partyMemberId,
                AssignedAt = DateTime.UtcNow,
                IsCompleted = false,
            }).ToList();


            foreach (var partyMember in partyMembers)
            {
                await _unitOfWork.QuestAssignment.CreateAsync(partyMember);
            }

            return Result.Success("Party members assigned to quest");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign members to quest.");
            return Result.Failure("An error occurred while assigning members to the quest.");
        }
    }
}