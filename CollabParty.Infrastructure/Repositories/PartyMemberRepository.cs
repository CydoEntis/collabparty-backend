using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class PartyMemberRepository : BaseRepository<PartyMember>, IPartyMemberRepository
{
    private readonly AppDbContext _db;

    public PartyMemberRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}