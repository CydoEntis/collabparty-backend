using AutoMapper;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class QuestStepService : IQuestStepService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestStepService> _logger;
    private readonly IMapper _mapper;


    public QuestStepService(IUnitOfWork unitOfWork, ILogger<QuestStepService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    public async Task<CreateQuestStepResponseDto> CreateQuestSteps(int questId, string[] steps)
    {
        try
        {
            var questSteps = steps.Select(step => new QuestStep
            {
                QuestId = questId,
                Description = step,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();


            foreach (var questStep in questSteps)
            {
                await _unitOfWork.QuestStep.CreateAsync(questStep);
            }

            return new CreateQuestStepResponseDto() { Message = "Created quest steps", QuestId = questId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party.");
            throw new ResourceCreationException("An error occured while trying to create quest steps");
        }
    }

    public async Task<UpdateQuestStepResponseDto> UpdateStepStatus(QuestStepStatusDto dto)
    {
        try
        {
            var questStep = await _unitOfWork.QuestStep.GetAsync(qs => qs.Id == dto.QuestStepId);
            if (EntityUtility.EntityIsNull(questStep))
                throw new NotFoundException("Quest step could not be found");


            if (questStep.IsCompleted)
                questStep.CompletedAt = DateTime.UtcNow;
            else
                questStep.CompletedAt = null;

            questStep.IsCompleted = dto.IsCompleted;
            questStep.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.QuestStep.UpdateAsync(questStep);

            return new UpdateQuestStepResponseDto()
                { Message = "Quest step status updated", QuestId = questStep.QuestId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update quest step with ID {dto.QuestStepId}.");
            throw new ResourceModificationException("An error occured while trying to update quest step status");
        }
    }

    public async Task<UpdateQuestStepResponseDto> UpdateQuestSteps(int questId, List<UpdateQuestStepDto> updatedSteps)
    {
        try
        {
            var existingSteps = await _unitOfWork.QuestStep.GetAllAsync(qs => qs.QuestId == questId);
            if (EntityUtility.EntityIsNull(existingSteps))
                throw new NotFoundException("Quest steps not found.");

            var stepsToAdd = new List<QuestStep>();
            var stepsToUpdate = new List<QuestStep>();
            var stepsToDelete = existingSteps.Where(es => updatedSteps.All(us => us.Id != es.Id)).ToList();

            foreach (var stepDto in updatedSteps)
            {
                if (stepDto.Id == 0 || stepDto.Id == null)
                {
                    stepsToAdd.Add(new QuestStep
                    {
                        QuestId = questId,
                        Description = stepDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    var existingStep = existingSteps.FirstOrDefault(es => es.Id == stepDto.Id);
                    if (existingStep != null)
                    {
                        existingStep.Description = stepDto.Description;
                        existingStep.UpdatedAt = DateTime.UtcNow;

                        stepsToUpdate.Add(existingStep);
                    }
                }
            }

            foreach (var step in stepsToAdd)
            {
                await _unitOfWork.QuestStep.CreateAsync(step);
            }

            foreach (var step in stepsToUpdate)
            {
                await _unitOfWork.QuestStep.UpdateAsync(step);
            }

            foreach (var step in stepsToDelete)
            {
                await _unitOfWork.QuestStep.RemoveAsync(step);
            }

            return new UpdateQuestStepResponseDto() { Message = "Quest steps updated", QuestId = questId, IsSuccess = true};
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update quest steps.");
            throw new ResourceModificationException("An error occured while trying to update quest steps.");
        }
    }


    public async Task<DeleteQuestStepResponseDto> RemoveQuestSteps(int questId, List<int> stepIdsToRemove)
    {
        try
        {
            var existingSteps = await _unitOfWork.QuestStep.GetAllAsync(qs => qs.QuestId == questId);

            if (EntityUtility.EntityIsNull(existingSteps) || !existingSteps.Any())
                throw new NotFoundException("Quest steps not found.");

            var stepsToDelete = existingSteps.Where(qs => stepIdsToRemove.Contains(qs.Id)).ToList();

            if (!stepsToDelete.Any())
                throw new NotFoundException("No quest steps to delete.");


            foreach (var step in stepsToDelete)
            {
                await _unitOfWork.QuestStep.RemoveAsync(step);
            }

            return new DeleteQuestStepResponseDto() { Message = "Quest steps removed", QuestId = questId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove quest steps.");
            throw new ResourceModificationException("An error occured while trying to delete quest steps.");
        }
    }
}