using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Services.Interfaces;

public interface IUserPartyService
{
    Task<Result<UserParty>> AssignUserAndRole(string userId, int partyId, UserRole role);
}