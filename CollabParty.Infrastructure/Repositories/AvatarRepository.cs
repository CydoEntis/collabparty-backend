using System.Linq.Expressions;
using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CollabParty.Infrastructure.Repositories;

public class AvatarRepository : BaseRepository<Avatar>, IAvatarRepository
{
    private readonly AppDbContext _db;

    public AvatarRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<int> CountAsync(Expression<Func<Avatar, bool>> filter = null)
    {
        IQueryable<Avatar> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync();
    }
}