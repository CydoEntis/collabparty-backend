

using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class AvatarRepository : BaseRepository<Avatar>, IAvatarRepository
{
    private readonly AppDbContext _db;

    public AvatarRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
 
}