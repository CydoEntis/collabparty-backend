using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestService
{
    Task<Result> CreateQuest(string userId, CreateQuestRequestDto dto);
    Task<Result<List<QuestResponseDto>>> GetQuests(string userId, int partyId);
}