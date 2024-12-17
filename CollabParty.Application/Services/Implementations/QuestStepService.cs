using AutoMapper;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;
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


    public async Task<Result> CreateQuestSteps(int questId, string[] steps)
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

            return Result.Success("Quest steps created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party.");
            return Result.Failure("An error occurred while creating the party.");
        }
    }
}