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
            // Map QueryParamsDto to QueryParams<Party>
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
                IncludeProperties = "PartyMembers,PartyMembers.User,PartyMembers.User.UnlockedAvatars,PartyMembers.User.UnlockedAvatars.Avatar",
                Filter = p => p.PartyMembers.Any(pm => pm.UserId == userId),
            };

            // Fetch paginated result
            var paginatedResult = await _unitOfWork.Party.GetPaginatedAsync(queryParams);

            // Map to PaginatedResult<PartyDto>

            var partyDto = _mapper.Map<List<PartyDto>>(paginatedResult.Items);

            var result = new PaginatedResult<PartyDto>(partyDto, paginatedResult.TotalItems,
                paginatedResult.CurrentPage, queryParams.PageSize);
            
            // Return success result
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
                includeProperties: "PartyMembers,PartyMembers.User,PartyMembers.User.UnlockedAvatars,PartyMembers.User.UnlockedAvatars.Avatar");


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
                includeProperties: "PartyMembers,PartyMembers.User,PartyMembers.User.UnlockedAvatars,PartyMembers.User.UnlockedAvatars.Avatar");

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
}