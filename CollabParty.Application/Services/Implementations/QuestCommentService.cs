using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Questlog.Application.Common.Models;

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


    public async Task<PaginatedResult<QuestCommentResponseDto>> GetPaginatedComments(
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
                IncludeProperties = "User.UnlockedAvatars.Avatar",
                Filter = qc => qc.QuestId == questId,
            };

            var paginatedResult = await _unitOfWork.QuestComment.GetPaginatedAsync(queryParams);

            var commentDtos = _mapper.Map<List<QuestCommentResponseDto>>(paginatedResult.Items);

            return new PaginatedResult<QuestCommentResponseDto>(
                commentDtos, paginatedResult.TotalItems, paginatedResult.CurrentPage, queryParams.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch paginated comments.");
            throw new FetchException("An error occured while fetching comments.");
        }
    }


    public async Task<AddQuestCommentResponseDto> AddComment(AddQuestCommentRequestDto dto)
    {
        try
        {
            var comment = _mapper.Map<QuestComment>(dto);

            await _unitOfWork.QuestComment.CreateAsync(comment);

            return new AddQuestCommentResponseDto() { Message = "Comment added successfully.", QuestId = dto.QuestId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add comment.");
            throw new ResourceCreationException("An error occured while adding comment.");
        }
    }


    public async Task<DeleteQuestCommentResponseDto> DeleteComment(int commentId, string userId)
    {
        try
        {
            var comment = await _unitOfWork.QuestComment.GetAsync(qc => qc.Id == commentId);
            var party = await _unitOfWork.Party.GetAsync(p => p.Quests.Any(q => q.Id == comment.QuestId));
            var partyMember =
                await _unitOfWork.PartyMember.GetAsync(pm => pm.UserId == userId && pm.PartyId == party.Id);
            if (EntityUtility.EntityIsNull(comment))
                throw new NotFoundException("Comment not found.");

            if (comment.UserId != userId && !RoleUtility.IsLeaderOrCaptain(partyMember))
                throw new PermissionException("You do not have permission to delete this comment.");


            await _unitOfWork.QuestComment.RemoveAsync(comment);

            return new DeleteQuestCommentResponseDto()
                { Message = "Comment removed successfully.", QuestId = comment.QuestId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete comment.");
            throw new ResourceModificationException("An error occured while deleting comment.");
        }
    }
}