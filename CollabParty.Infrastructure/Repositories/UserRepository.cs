﻿using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;
namespace CollabParty.Infrastructure.Repositories;

public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.ApplicationUsers.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}