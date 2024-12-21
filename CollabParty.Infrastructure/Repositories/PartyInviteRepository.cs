using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class PartyInviteRepository : BaseRepository<PartyInvite>, IPartyInviteRepository
{
    private readonly AppDbContext _db;

    public PartyInviteRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}