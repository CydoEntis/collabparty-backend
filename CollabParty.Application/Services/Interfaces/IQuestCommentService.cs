// using CollabParty.Application.Common.Dtos;
// using CollabParty.Application.Common.Dtos.QuestComments;
// using CollabParty.Application.Common.Models;
// using Questlog.Application.Common.Models;
//
// namespace CollabParty.Application.Services.Interfaces;
//
// public interface IQuestCommentService
// {
//     Task<Result<PaginatedResult<QuestCommentResponseDto>>> GetPaginatedComments(
//         int questId, QueryParamsDto dto);
//
//     Task<Result<int>> AddComment(AddCommentRequestDto dto);
//     Task<Result<int>> EditComment(EditCommentRequestDto dto);
//     Task<Result<int>> DeleteComment(int commentId, string userId);
// }