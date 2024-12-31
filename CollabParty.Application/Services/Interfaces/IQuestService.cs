using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestService
{
    Task<CreateQuestResponseDto> CreateQuest(string userId, CreateQuestRequestDto dto);

    Task<PaginatedResult<QuestResponseDto>> GetQuests(string userId, int partyId,
        QueryParamsDto dto);

    Task<QuestDetailResponseDto> GetQuest(int questId);
    Task<CompleteQuestResponseDto> CompleteQuest(string userId, int questId);
    Task<UpdateQuestResponseDto> UpdateQuest(string userId, int questId, UpdateQuestRequestDto dto);
    Task<DeleteQuestResponseDto> DeleteQuest(string userId, int questId);
}