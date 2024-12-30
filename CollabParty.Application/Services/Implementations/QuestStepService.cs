// using AutoMapper;
// using CollabParty.Application.Common.Dtos.Quest;
// using CollabParty.Application.Common.Dtos.QuestSteps;
// using CollabParty.Application.Common.Models;
// using CollabParty.Application.Services.Interfaces;
// using CollabParty.Domain.Entities;
// using CollabParty.Domain.Interfaces;
// using Microsoft.Extensions.Logging;
//
// namespace CollabParty.Application.Services.Implementations;
//
// public class QuestStepService : IQuestStepService
// {
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly ILogger<QuestStepService> _logger;
//     private readonly IMapper _mapper;
//
//
//     public QuestStepService(IUnitOfWork unitOfWork, ILogger<QuestStepService> logger, IMapper mapper)
//     {
//         _unitOfWork = unitOfWork;
//         _logger = logger;
//         _mapper = mapper;
//     }
//
//
//     public async Task<Result> CreateQuestSteps(int questId, string[] steps)
//     {
//         try
//         {
//             var questSteps = steps.Select(step => new QuestStep
//             {
//                 QuestId = questId,
//                 Description = step,
//                 IsCompleted = false,
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             }).ToList();
//
//
//             foreach (var questStep in questSteps)
//             {
//                 await _unitOfWork.QuestStep.CreateAsync(questStep);
//             }
//
//             return Result.Success("Quest steps created successfully.");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Failed to assign user to party.");
//             return Result.Failure("An error occurred while creating the party.");
//         }
//     }
//
//     public async Task<Result<int>> UpdateStepStatus(QuestStepStatusDto dto)
//     {
//         try
//         {
//             var questStep = await _unitOfWork.QuestStep.GetAsync(qs => qs.Id == dto.QuestStepId);
//             if (questStep == null)
//             {
//                 return Result<int>.Failure($"Quest step with ID {dto.QuestStepId} not found.");
//             }
//
//             if (questStep.IsCompleted)
//             {
//                 questStep.CompletedAt = DateTime.UtcNow;
//             }
//             else
//             {
//                 questStep.CompletedAt = null;
//             }
//
//             questStep.IsCompleted = dto.IsCompleted;
//             questStep.UpdatedAt = DateTime.UtcNow;
//
//             await _unitOfWork.QuestStep.UpdateAsync(questStep);
//
//             return Result<int>.Success(questStep.QuestId);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, $"Failed to update quest step with ID {dto.QuestStepId}.");
//             return Result<int>.Failure("An error occurred while updating the quest step.");
//         }
//     }
//
//     public async Task<Result> UpdateQuestSteps(int questId, List<UpdateQuestStepDto> updatedSteps)
//     {
//         try
//         {
//             var existingSteps = await _unitOfWork.QuestStep.GetAllAsync(qs => qs.QuestId == questId);
//             if (existingSteps == null)
//             {
//                 return Result.Failure($"No quest steps found for quest ID {questId}.");
//             }
//
//             var stepsToAdd = new List<QuestStep>();
//             var stepsToUpdate = new List<QuestStep>();
//             var stepsToDelete = existingSteps.Where(es => updatedSteps.All(us => us.Id != es.Id)).ToList();
//
//             foreach (var stepDto in updatedSteps)
//             {
//                 if (stepDto.Id == 0)
//                 {
//                     stepsToAdd.Add(new QuestStep
//                     {
//                         QuestId = questId,
//                         Description = stepDto.Description,
//                         CreatedAt = DateTime.UtcNow,
//                         UpdatedAt = DateTime.UtcNow
//                     });
//                 }
//                 else
//                 {
//                     var existingStep = existingSteps.FirstOrDefault(es => es.Id == stepDto.Id);
//                     if (existingStep != null)
//                     {
//                         existingStep.Description = stepDto.Description;
//                         existingStep.UpdatedAt = DateTime.UtcNow;
//
//                         stepsToUpdate.Add(existingStep);
//                     }
//                 }
//             }
//
//             foreach (var step in stepsToAdd)
//             {
//                 await _unitOfWork.QuestStep.CreateAsync(step);
//             }
//
//             foreach (var step in stepsToUpdate)
//             {
//                 await _unitOfWork.QuestStep.UpdateAsync(step);
//             }
//
//             foreach (var step in stepsToDelete)
//             {
//                 await _unitOfWork.QuestStep.RemoveAsync(step);
//             }
//
//             return Result.Success("Quest steps updated successfully.");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Failed to update quest steps.");
//             return Result.Failure("An error occurred while updating quest steps.");
//         }
//     }
//
//
//     public async Task<Result> RemoveQuestSteps(int questId, List<int> stepIdsToRemove)
//     {
//         try
//         {
//             var existingSteps = await _unitOfWork.QuestStep.GetAllAsync(qs => qs.QuestId == questId);
//
//             if (existingSteps == null || !existingSteps.Any())
//             {
//                 return Result.Failure($"No quest steps found for quest ID {questId}.");
//             }
//
//             var stepsToDelete = existingSteps.Where(qs => stepIdsToRemove.Contains(qs.Id)).ToList();
//
//             if (!stepsToDelete.Any())
//             {
//                 return Result.Failure("No matching quest steps found to remove.");
//             }
//
//             foreach (var step in stepsToDelete)
//             {
//                 await _unitOfWork.QuestStep.RemoveAsync(step);
//             }
//
//             return Result.Success("Quest steps removed successfully.");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Failed to remove quest steps.");
//             return Result.Failure("An error occurred while removing quest steps.");
//         }
//     }
// }