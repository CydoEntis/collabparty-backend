using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Implementations;

public class PartyService : IPartyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IPartyMemberService _partyMemberService;


    public PartyService(IUnitOfWork unitOfWork, ILogger<Party> logger, IMapper mapper,
        IPartyMemberService partyMemberService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _partyMemberService = partyMemberService;
    }

    public async Task<Result<PartyDto>> CreateParty(string userId, CreatePartyDto dto)
    {
        try
        {
            var newParty = _mapper.Map<Party>(dto);
            newParty.CreatedById = userId;
            Party createdParty = await _unitOfWork.Party.CreateAsync(newParty);

            await _partyMemberService.AddPartyLeader(userId, createdParty.Id);

            var foundParty = await _unitOfWork.Party.GetAsync(p => p.Id == newParty.Id,
                includeProperties: "CreatedBy,CreatedBy.UnlockedAvatars.Avatar");

            var partyDto = _mapper.Map<PartyDto>(foundParty);
            return Result<PartyDto>.Success(partyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party.");
            return Result<PartyDto>.Failure("An error occurred while creating the party.");
        }
    }


    public async Task<Result<PaginatedResult<PartyDto>>> GetAllPartiesForUser(string userId, QueryParamsDto dto)
    {
        try
        {
            var queryParams = new QueryParams<Party>
            {
                Search = dto.Search,
                OrderBy = dto.OrderBy,
                SortBy = dto.SortBy,
                DateFilter = dto.DateFilter,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                IncludeProperties = "Quests,PartyMembers.User.UnlockedAvatars.Avatar",
                Filter = p => p.PartyMembers.Any(pm => pm.UserId == userId),
            };

            var paginatedResult = await _unitOfWork.Party.GetPaginatedAsync(queryParams);

            foreach (var party in paginatedResult.Items)
            {
                party.Quests ??= new List<Quest>();
                party.Quests = party.Quests.Where(q => q.PartyId == party.Id).ToList();
            }

            var partyDto = _mapper.Map<List<PartyDto>>(paginatedResult.Items);

            var result = new PaginatedResult<PartyDto>(partyDto, paginatedResult.TotalItems,
                paginatedResult.CurrentPage, queryParams.PageSize);

            return Result<PaginatedResult<PartyDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user parties.");
            return Result<PaginatedResult<PartyDto>>.Failure("An error occurred while fetching parties.");
        }
    }


    public async Task<Result<List<PartyDto>>> GetRecentParties(string userId)
    {
        try
        {
            var recentParties = await _unitOfWork.Party.GetMostRecentPartiesForUserAsync(userId,
                includeProperties:
                "PartyMembers,PartyMembers.User,PartyMembers.User.UnlockedAvatars,PartyMembers.User.UnlockedAvatars.Avatar");


            var partyDto = _mapper.Map<List<PartyDto>>(recentParties);
            return Result<List<PartyDto>>.Success(partyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party.");
            return Result<List<PartyDto>>.Failure("An error occurred while creating party.");
        }
    }

    public async Task<Result<PartyDto>> GetParty(string userId, int partyId)
    {
        try
        {
            var foundParty = await _unitOfWork.Party.GetAsync(
                p => p.Id == partyId && p.PartyMembers.Any(pm => pm.UserId == userId),
                includeProperties:
                "PartyMembers.User.UnlockedAvatars.Avatar");

            if (foundParty == null)
                return Result<PartyDto>.Failure($"No party with the {partyId} exists");

            var partyDto = _mapper.Map<PartyDto>(foundParty);
            return Result<PartyDto>.Success(partyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party with id");
            return Result<PartyDto>.Failure("An error occurred while fetching party.");
        }
    }

    public async Task<Result<int>> UpdateParty(string userId, int partyId, UpdatePartyDto dto)
    {
        try
        {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId);

            if (user.Role is not UserRole.Leader)
            {
                return Result<int>.Failure("You do not have permission to update party.");
            }


            var existingParty = await _unitOfWork.Party.GetAsync(p => p.Id == partyId && p.CreatedById == userId,
                includeProperties: "PartyMembers");

            if (existingParty == null)
            {
                return Result<int>.Failure("Party not found or you do not have permission to update this party.");
            }

            existingParty.Name = dto.PartyName;
            existingParty.Description = dto.Description;
            existingParty.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Party.UpdateAsync(existingParty);


            return Result<int>.Success(existingParty.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update party.");
            return Result<int>.Failure("An error occurred while updating the party.");
        }
    }

    public async Task<Result<int>> DeleteParty(string userId, int partyId)
    {
        try
        {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId);

            if (user.Role is not UserRole.Leader)
            {
                return Result<int>.Failure("You do not have permission to delete party.");
            }

            var existingParty = await _unitOfWork.Party.GetAsync(p => p.Id == partyId && p.CreatedById == userId,
                includeProperties: "Quests,QuestAssignments,QuestFiles,QuestComments,PartyMembers");

            if (existingParty == null)
            {
                return Result<int>.Failure("Party not found or you do not have permission to delete this party.");
            }

            foreach (var quest in existingParty.Quests.ToList())
            {
                foreach (var assignment in quest.QuestAssignments.ToList())
                {
                    await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
                }

                foreach (var file in quest.QuestFiles.ToList())
                {
                    await _unitOfWork.QuestFile.RemoveAsync(file);
                }

                foreach (var comment in quest.QuestComments.ToList())
                {
                    await _unitOfWork.QuestComment.RemoveAsync(comment);
                }

                await _unitOfWork.Quest.RemoveAsync(quest);
            }

            foreach (var partyMember in existingParty.PartyMembers.ToList())
            {
                await _unitOfWork.PartyMember.RemoveAsync(partyMember);
            }

            await _unitOfWork.Party.RemoveAsync(existingParty);

            return Result<int>.Success(partyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete party.");
            return Result<int>.Failure("An error occurred while deleting the party.");
        }
    }
}