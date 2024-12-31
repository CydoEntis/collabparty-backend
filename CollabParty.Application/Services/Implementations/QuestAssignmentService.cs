using AutoMapper;
using CollabParty.Application.Common.Dtos.QuestAssignment;
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

    public async Task<AssignPartyMembersResponseDto> AssignPartyMembersToQuest(int questId, string[] partyMemberIds)
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

    public async Task<UpdateAssignedPartyMembersResponseDto> UpdateQuestAssignments(int questId,
        string[] updatedPartyMemberIds)
    {
        try
        {
            var existingAssignments = await _unitOfWork.QuestAssignment.GetAllAsync(qa => qa.QuestId == questId);

            if (existingAssignments == null)
            {
                return Result.Failure($"No quest assignments found for quest ID {questId}.");
            }

            var assignmentsToDelete = existingAssignments
                .Where(assignment => !updatedPartyMemberIds.Contains(assignment.UserId))
                .ToList();

            var assignmentsToAdd = updatedPartyMemberIds
                .Where(userId => !existingAssignments.Any(assignment => assignment.UserId == userId))
                .Select(userId => new QuestAssignment
                {
                    QuestId = questId,
                    UserId = userId,
                    AssignedAt = DateTime.UtcNow,
                    IsCompleted = false
                })
                .ToList();

            foreach (var assignment in assignmentsToDelete)
            {
                await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
            }

            foreach (var newAssignment in assignmentsToAdd)
            {
                await _unitOfWork.QuestAssignment.CreateAsync(newAssignment);
            }

            return Result.Success("Quest assignments updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update quest assignments.");
            return Result.Failure("An error occurred while updating quest assignments.");
        }
    }

    public async Task<DeleteAssignedPartyMembersResponseDto> DeleteQuestAssignments(int questId,
        string[] partyMemberIdsToDelete)
    {
        try
        {
            var existingAssignments = await _unitOfWork.QuestAssignment.GetAllAsync(qa => qa.QuestId == questId);

            if (existingAssignments == null || !existingAssignments.Any())
            {
                return Result.Failure($"No quest assignments found for quest ID {questId}.");
            }

            var assignmentsToDelete = existingAssignments
                .Where(assignment => partyMemberIdsToDelete.Contains(assignment.UserId))
                .ToList();

            if (!assignmentsToDelete.Any())
            {
                return Result.Failure("No matching quest assignments found to delete.");
            }

            foreach (var assignment in assignmentsToDelete)
            {
                await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
            }

            return Result.Success("Quest assignments deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete quest assignments.");
            return Result.Failure("An error occurred while deleting quest assignments.");
        }
    }
}