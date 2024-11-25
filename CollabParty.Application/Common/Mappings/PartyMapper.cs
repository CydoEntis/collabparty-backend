using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public static class PartyMapper
{
    public static Party FromCreatePartyDto(CreatePartyDto dto)
    {
        return new Party
        {
            PartyName = dto.PartyName,
        };
    }

    public static PartyDto ToPartyDto(Party party)
    {
        return new PartyDto
        {
            Id = party.Id,
            PartyName = party.PartyName,
            CreatedAt = party.CreatedAt,
            UpdatedAt = party.UpdatedAt,
            Members = party.UserParties.Select(ua => ua.User).Select(user => UserMapper.ToUserDto(user)).ToList()
        };
    }

    public static PartyDto ToPartyDto(UserParty userParty)
    {
        return new PartyDto
        {
            Id = userParty.Party.Id,
            PartyName = userParty.Party.PartyName,
            CreatedAt = userParty.Party.CreatedAt,
            UpdatedAt = userParty.Party.UpdatedAt,
            Members = userParty.Party.UserParties
                .Select(up => UserMapper.ToUserDto(up.User))
                .ToList()
        };
    }
}