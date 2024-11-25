using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Services.Interfaces;

public interface IUserPartyService
{
    Task<Result> AssignUserAndRole(string userId, int partyId, UserRole role);
    Task<Result<List<PartyDto>>> GetAllPartiesForUser(string userId);
    Task<Result<PartyDto>> GetParty(string userId, int partyId);
    Task<Result<List<UserDto>>> GetPartyMembers(string userId, int partyId);
    Task<Result<List<UserDto>>> RemovePartyMembers(string userId, int partyId, RemoverUserFromPartyDto dto);
}