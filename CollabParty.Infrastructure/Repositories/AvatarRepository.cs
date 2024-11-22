using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class AvatarRepository : BaseRepository<Avatar>, IAvatarRepository
{
    private readonly AppDbContext _db;

    public AvatarRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}