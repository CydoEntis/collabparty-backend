using AutoMapper;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class QuestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestService> _logger;
    private readonly IMapper _mapper;

    public QuestService(IUnitOfWork unitOfWork, ILogger<QuestService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<QuestResponseDto>> CreateQuest(string userId, CreateQuestRequestDto dto)
    {
        try
        {
            var newQuest = _mapper.Map<Quest>(dto);
            newQuest.CreatedById = userId;
            Quest createdQuest = await _unitOfWork.Quest.CreateAsync(newQuest);

            var questDto = _mapper.Map<QuestResponseDto>(createdQuest);

            return Result<QuestResponseDto>.Success(questDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party.");
            return Result<QuestResponseDto>.Failure("An error occurred while creating the party.");
        }
    }
}