using AutoMapper;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class QuestCommentService : IQuestCommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestCommentService> _logger;
    private readonly IMapper _mapper;

    public QuestCommentService(IUnitOfWork unitOfWork, ILogger<QuestCommentService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    public async Task<Result<PaginatedResult<QuestCommentResponseDto>>> GetPaginatedComments(
        int questId, QueryParamsDto dto)
    {
        try
        {
            var queryParams = new QueryParams<QuestComment>
            {
                Search = dto.Search,
                OrderBy = dto.OrderBy,
                SortBy = dto.SortBy,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                IncludeProperties = "User", // Include related user if needed
                Filter = qc => qc.QuestId == questId,
            };

            var paginatedResult = await _unitOfWork.QuestComment.GetPaginatedAsync(queryParams);

            var commentDtos = _mapper.Map<List<QuestCommentResponseDto>>(paginatedResult.Items);

            var result = new PaginatedResult<QuestCommentResponseDto>(
                commentDtos, paginatedResult.TotalItems, paginatedResult.CurrentPage, queryParams.PageSize);

            return Result<PaginatedResult<QuestCommentResponseDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch paginated comments.");
            return Result<PaginatedResult<QuestCommentResponseDto>>.Failure(
                "An error occurred while fetching comments.");
        }
    }


    public async Task<Result<int>> AddComment(AddCommentRequestDto dto)
    {
        try
        {
            var comment = _mapper.Map<QuestComment>(dto);

            await _unitOfWork.QuestComment.CreateAsync(comment);

            return Result<int>.Success(comment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add comment.");
            return Result<int>.Failure("An error occurred while adding the comment.");
        }
    }

    public async Task<Result> EditComment(EditCommentRequestDto dto)
    {
        try
        {
            var comment = await _unitOfWork.QuestComment.GetAsync(qc => qc.Id == dto.Id);

            if (comment == null)
                return Result.Failure("Comment not found.");

            if (comment.UserId != dto.UserId)
                return Result.Failure("You do not have permission to edit this comment.");

            comment.Content = dto.Content;
            await _unitOfWork.QuestComment.UpdateAsync(comment);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to edit comment.");
            return Result.Failure("An error occurred while editing the comment.");
        }
    }

    public async Task<Result> DeleteComment(int commentId, string userId)
    {
        try
        {
            var comment = await _unitOfWork.QuestComment.GetAsync(qc => qc.Id == commentId);

            if (comment == null)
                return Result.Failure("Comment not found.");

            if (comment.UserId != userId)
                return Result.Failure("You do not have permission to delete this comment.");

            await _unitOfWork.QuestComment.RemoveAsync(comment);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete comment.");
            return Result.Failure("An error occurred while deleting the comment.");
        }
    }
}