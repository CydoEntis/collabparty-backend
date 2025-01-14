﻿using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
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

    public async Task<PartyResponseDto> CreateParty(string userId, CreatePartyRequestDto requestDto)
    {
            var newParty = _mapper.Map<Party>(requestDto);
            newParty.CreatedById = userId;
            newParty.CreatedAt = DateTime.UtcNow;
            Party createdParty = await _unitOfWork.Party.CreateAsync(newParty);

            await _partyMemberService.AddPartyLeader(userId, createdParty.Id);

            var foundParty = await _unitOfWork.Party.GetAsync(p => p.Id == newParty.Id,
                includeProperties: "CreatedBy,CreatedBy.UnlockedAvatars.Avatar");

            var partyDto = _mapper.Map<PartyResponseDto>(foundParty);
            return partyDto;
    }


    public async Task<PaginatedResult<PartyResponseDto>> GetAllPartiesForUser(string userId, QueryParamsDto dto)
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
                IncludeProperties = "PartyMembers.User.UnlockedAvatars.Avatar",
                Filter = p => p.PartyMembers.Any(pm => pm.UserId == userId),
            };

            var paginatedResult = await _unitOfWork.Party.GetPaginatedAsync(queryParams);

            var partyIds = paginatedResult.Items.Select(p => p.Id).ToList();
            var quests = await _unitOfWork.Quest.GetAllAsync(q => partyIds.Contains(q.PartyId));

            var partyDto = _mapper.Map<List<PartyResponseDto>>(paginatedResult.Items);

            foreach (var party in partyDto)
            {
                var partyQuests = quests.Where(q => q.PartyId == party.Id).ToList();

                party.TotalQuests = partyQuests.Count;
                party.CompletedQuests = partyQuests.Count(q => q.IsCompleted);
                party.PastDueQuests = partyQuests.Count(q => q.DueDate < DateTime.UtcNow);
            }

            var result = new PaginatedResult<PartyResponseDto>(
                partyDto,
                paginatedResult.TotalItems,
                paginatedResult.CurrentPage,
                queryParams.PageSize
            );

            return result;
    }


    public async Task<List<PartyResponseDto>> GetRecentParties(string userId)
    {
            var recentParties = await _unitOfWork.Party.GetMostRecentPartiesForUserAsync(userId,
                includeProperties:
                "PartyMembers,PartyMembers.User,PartyMembers.User.UnlockedAvatars,PartyMembers.User.UnlockedAvatars.Avatar");


            var partyDtos = _mapper.Map<List<PartyResponseDto>>(recentParties);
            return partyDtos;
    }

    public async Task<PartyResponseDto> GetParty(string userId, int partyId)
    {
            var foundParty = await _unitOfWork.Party.GetAsync(
                p => p.Id == partyId && p.PartyMembers.Any(pm => pm.PartyId == partyId),
                includeProperties: "PartyMembers.User.UnlockedAvatars.Avatar");

            if (EntityUtility.EntityIsNull(foundParty))
                throw new NotFoundException($"Party with id {partyId} not found.");

            var currentUserPartyMember = foundParty.PartyMembers
                .FirstOrDefault(pm => pm.UserId == userId);


            if (EntityUtility.EntityIsNull(currentUserPartyMember))
                throw new NotFoundException("User is not a member of this party.");

            var partyDto = _mapper.Map<PartyResponseDto>(foundParty);

            partyDto.CurrentUserRole = currentUserPartyMember.Role;

            return partyDto;
    }


    public async Task<UpdatePartyResponseDto> UpdateParty(string userId, int partyId, UpdatePartyRequestDto requestDto)
    {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId && p.PartyId == partyId);

            if (!await IsLeaderAsync(userId, partyId))
                throw new PermissionException("You do not have permission to delete the party.");


            var existingParty = await _unitOfWork.Party.GetAsync(p => p.Id == partyId,
                includeProperties: "PartyMembers");

            if (EntityUtility.EntityIsNull(existingParty))
                throw new NotFoundException($"Party with id {partyId} not found.");

            existingParty.Name = requestDto.Name;
            existingParty.Description = requestDto.Description;
            existingParty.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Party.UpdateAsync(existingParty);


            return new UpdatePartyResponseDto() { Message = "Party updated successfully", PartyId = partyId };
    }


    public async Task<DeletePartyResponseDto> DeleteParty(string userId, int partyId)
    {
            var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId && p.PartyId == partyId);


            if (!await IsLeaderAsync(userId, partyId))
                throw new PermissionException("You do not have permission to delete party.");


            var existingParty = await _unitOfWork.Party.GetAsync(p => p.Id == partyId && p.CreatedById == userId,
                includeProperties:
                "Quests.QuestAssignments,Quests.QuestFiles,Quests.QuestComments,Quests.QuestSteps,PartyMembers");

            if (EntityUtility.EntityIsNull(existingParty))
                throw new NotFoundException($"Party with id {partyId} not found.");

            foreach (var quest in existingParty.Quests.ToList())
            {
                await DeleteEntitiesAsync(quest.QuestAssignments.ToList(), _unitOfWork.QuestAssignment.RemoveAsync);
                await DeleteEntitiesAsync(quest.QuestFiles.ToList(), _unitOfWork.QuestFile.RemoveAsync);
                await DeleteEntitiesAsync(quest.QuestComments.ToList(), _unitOfWork.QuestComment.RemoveAsync);
                await DeleteEntitiesAsync(quest.QuestSteps.ToList(), _unitOfWork.QuestStep.RemoveAsync);

                await _unitOfWork.Quest.RemoveAsync(quest);
            }

            foreach (var partyMember in existingParty.PartyMembers.ToList())
            {
                await _unitOfWork.PartyMember.RemoveAsync(partyMember);
            }

            await _unitOfWork.Party.RemoveAsync(existingParty);

            return new DeletePartyResponseDto() { Message = "Party deleted successfully", PartyId = partyId };
    }

    private async Task DeleteEntitiesAsync<T>(List<T> entities, Func<T, Task> removeMethod)
    {
        foreach (var entity in entities)
        {
            await removeMethod(entity);
        }
    }

    private async Task<bool> IsLeaderAsync(string userId, int partyId)
    {
        var user = await _unitOfWork.PartyMember.GetAsync(p => p.UserId == userId && p.PartyId == partyId);

        return RoleUtility.IsLeader(user);
    }
}