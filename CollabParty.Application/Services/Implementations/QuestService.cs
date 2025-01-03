using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
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

    public async Task<CreateQuestResponseDto> CreateQuest(string userId, CreateQuestRequestDto dto)
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

            return new CreateQuestResponseDto()
                { Message = "Quest created successfully.", QuestId = createdQuest.Id, PartyId = createdQuest.PartyId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create quest.");
            throw new ResourceCreationException("An error occured while creating quest.");
        }
    }

    public async Task<PaginatedResult<QuestResponseDto>> GetQuests(string userId, int partyId,
        QueryParamsDto dto)
    {
        try
        {
            var partyMember = await _unitOfWork.PartyMember
                .GetAsync(pm => pm.UserId == userId && pm.PartyId == partyId);

            if (EntityUtility.EntityIsNull(partyMember))
                throw new NotFoundException("Party Member does not exist.");

            var isLeaderOrCaptain = partyMember.Role == UserRole.Leader || partyMember.Role == UserRole.Captain;

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
                    "QuestAssignments.User.UnlockedAvatars.Avatar,QuestSteps,QuestComments",
                Filter = q =>
                    q.PartyId == partyId && !q.IsCompleted &&
                    (isLeaderOrCaptain || q.QuestAssignments.Any(qa => qa.UserId == userId)),
            };

            var paginatedResult = await _unitOfWork.Quest.GetPaginatedAsync(queryParams);

            var questDtos = _mapper.Map<List<QuestResponseDto>>(paginatedResult.Items);

            return new PaginatedResult<QuestResponseDto>(questDtos, paginatedResult.TotalItems,
                paginatedResult.CurrentPage, queryParams.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user quests.");
            throw new FetchException("An error occured while fetching quests.");
        }
    }


    public async Task<QuestDetailResponseDto> GetQuest(int questId)
    {
        try
        {
            var foundQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties:
                "QuestAssignments.User.UnlockedAvatars.Avatar,QuestSteps,QuestComments,QuestFiles");


            if (EntityUtility.EntityIsNull(foundQuest))
                throw new NotFoundException("Quest does not exist.");

            var partyMembers = await _unitOfWork.PartyMember.GetAllAsync(p => p.PartyId == foundQuest.PartyId,
                includeProperties: "User.UnlockedAvatars.Avatar");
            var partyMembersDto = _mapper.Map<List<PartyMemberResponseDto>>(partyMembers);

            var questDetailDto = _mapper.Map<QuestDetailResponseDto>(foundQuest);
            questDetailDto.PartyMembers = partyMembersDto;
            return questDetailDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party with id");
            throw new FetchException("An error occured while fetching quest.");
        }
    }

    public async Task<CompleteQuestResponseDto> CompleteQuest(string userId, int questId)
    {
        try
        {
            var foundQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties:
                "Party.PartyMembers.User.UnlockedAvatars.Avatar,QuestSteps,QuestComments,QuestFiles");

            if (EntityUtility.EntityIsNull(foundQuest))
                throw new NotFoundException("Quest does not exist.");

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

            return new CompleteQuestResponseDto()
                { Message = "Quest created successfully.", QuestId = foundQuest.Id, PartyId = foundQuest.PartyId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party with id");
            throw new OperationException("Quest Completion Exception", "An error occured while completing quest.");
        }
    }

    public async Task<UpdateQuestResponseDto> UpdateQuest(string userId, int questId, UpdateQuestRequestDto dto)
    {
        try
        {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId);


            if (RoleUtility.IsLeaderOrCaptain(user))
                throw new PermissionException("You do not have permission to update quests.");


            var existingQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties: "QuestSteps,QuestAssignments"
            );

            if (EntityUtility.EntityIsNull(existingQuest))
                throw new NotFoundException("Quest does not exist.");


            existingQuest.Name = dto.Name;
            existingQuest.Description = dto.Description;
            existingQuest.PriorityLevel = dto.PriorityLevel;
            existingQuest.DueDate = dto.DueDate;

            existingQuest.ExpReward = CalculateQuestExpReward(dto.PriorityLevel);
            existingQuest.GoldReward = CalculateQuestGoldReward(dto.PriorityLevel);

            existingQuest.UpdatedAt = DateTime.UtcNow;

            if (dto.Steps != null && dto.Steps.Any())
            {
                var stepUpdateResult = await _questStepService.UpdateQuestSteps(existingQuest.Id, dto.Steps);
                if (!stepUpdateResult.IsSuccess)
                    throw new ResourceModificationException("Updating quest steps failed.");
            }

            if (!EntityUtility.EntityIsNull(dto.AssignedPartyMembers))
            {
                var assignmentUpdateResult =
                    await _questAssignmentService.UpdateQuestAssignments(existingQuest.Id, dto.AssignedPartyMembers);
                if (!assignmentUpdateResult.IsSuccess)
                    throw new ResourceModificationException("Updating quest assignment failed.");
            }

            await _unitOfWork.Quest.UpdateAsync(existingQuest);

            return new UpdateQuestResponseDto()
                { Message = "Quest updated successfully", QuestId = existingQuest.Id, PartyId = existingQuest.PartyId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update quest.");
            throw new ResourceModificationException("An error occured while updating quest.");
        }
    }


    public async Task<DeleteQuestResponseDto> DeleteQuest(string userId, int questId)
    {
        try
        {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId);

            if (RoleUtility.IsLeaderOrCaptain(user))
                throw new PermissionException("You do not have permission to delete this quest.");

            var existingQuest = await _unitOfWork.Quest.GetAsync(
                q => q.Id == questId,
                includeProperties: "QuestSteps,QuestAssignments,QuestComments"
            );

            if (EntityUtility.EntityIsNull(existingQuest))
                throw new NotFoundException("Quest does not exist.");

            var commentsToDelete = await _unitOfWork.QuestComment.GetAllAsync(qc => qc.QuestId == questId);
            foreach (var comment in commentsToDelete)
            {
                await _unitOfWork.QuestComment.RemoveAsync(comment);
            }

            foreach (var step in existingQuest.QuestSteps.ToList())
            {
                await _unitOfWork.QuestStep.RemoveAsync(step);
            }

            foreach (var assignment in existingQuest.QuestAssignments.ToList())
            {
                await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
            }

            await _unitOfWork.Quest.RemoveAsync(existingQuest);

            return new DeleteQuestResponseDto()
            {
                Message = "Quest deleted successfully",
                QuestId = existingQuest.Id,
                PartyId = existingQuest.PartyId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete quest.");
            throw new ResourceModificationException("An error occurred while deleting the quest.");
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