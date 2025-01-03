using AutoMapper;
using CollabParty.Application.Common.Dtos.QuestAssignment;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
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
            var validUserIds = await _unitOfWork.User.GetAllAsync(u => partyMemberIds.Contains(u.Id));

            if (validUserIds.Count() != partyMemberIds.Length)
            { 
                throw new ResourceCreationException("Some party members are invalid or do not exist.");
            }

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

            return new AssignPartyMembersResponseDto()
            {
                Message = "Party members assigned to quest",
                QuestId = questId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign members to quest.");
            throw new ResourceCreationException("An error occurred while assigning members to the quest.");
        }
    }


    public async Task<UpdateAssignedPartyMembersResponseDto> UpdateQuestAssignments(int questId,
        string[] updatedPartyMemberIds)
    {
        try
        {
            var existingAssignments = await _unitOfWork.QuestAssignment.GetAllAsync(qa => qa.QuestId == questId);

            if (EntityUtility.EntityIsNull(existingAssignments))
                throw new NotFoundException("Quest assignments not found");

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

            return new UpdateAssignedPartyMembersResponseDto()
                { Message = "Quest assignments updated", QuestId = questId, IsSuccess = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update quest assignments.");
            throw new ResourceModificationException("An error occured while updating quest assignments.");
        }
    }

    public async Task<DeleteAssignedPartyMembersResponseDto> DeleteQuestAssignments(int questId,
        string[] partyMemberIdsToDelete)
    {
        try
        {
            var existingAssignments = await _unitOfWork.QuestAssignment.GetAllAsync(qa => qa.QuestId == questId);

            if (existingAssignments == null || !existingAssignments.Any())
                throw new NotFoundException("Quest assignments not found");


            var assignmentsToDelete = existingAssignments
                .Where(assignment => partyMemberIdsToDelete.Contains(assignment.UserId))
                .ToList();

            if (!assignmentsToDelete.Any())
                throw new NotFoundException("No quest assignments for deletion could be found.");


            foreach (var assignment in assignmentsToDelete)
            {
                await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
            }

            return new DeleteAssignedPartyMembersResponseDto()
                { Message = "Quest assignments deleted", QuestId = questId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete quest assignments.");
            throw new ResourceModificationException("An error occured while deleting quest assignments.");
        }
    }
}