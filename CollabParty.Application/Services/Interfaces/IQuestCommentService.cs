using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestCommentService
{
    Task<Result<int>> AddComment(AddCommentRequestDto dto);
    Task<Result> EditComment(EditCommentRequestDto dto);
    Task<Result> DeleteComment(int commentId, string userId);
}