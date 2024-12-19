using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestService
{
    Task<Result> CreateQuest(string userId, CreateQuestRequestDto dto);

    Task<Result<PaginatedResult<QuestResponseDto>>> GetQuests(string userId, int partyId,
        QueryParamsDto dto);

    Task<Result<QuestDetailResponseDto>> GetQuest(int questId);
    Task<Result<int>> CompleteQuest(string userId, int questId);
}