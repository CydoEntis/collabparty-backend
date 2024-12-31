using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Models;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestCommentService
{
    Task<PaginatedResult<QuestCommentResponseDto>> GetPaginatedComments(
        int questId, QueryParamsDto dto);

    Task<AddQuestCommentResponseDto> AddComment(AddQuestCommentRequestDto dto);
    Task<DeleteQuestCommentResponseDto> DeleteComment(int commentId, string userId);
}