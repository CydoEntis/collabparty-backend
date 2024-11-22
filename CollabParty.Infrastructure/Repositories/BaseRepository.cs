using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Application.Common.Models;

namespace Questlog.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly AppDbContext _db;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveAsync();

        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null)
    {
        if (_dbSet == null)
        {
            throw new InvalidOperationException("DbSet is not initialized.");
        }

        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp.Trim());
            }
        }

        return await query.ToListAsync();
    }


    public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true,
        string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }


    protected IQueryable<T> ApplyIncludeProperties(IQueryable<T> query, string includeProperties)
    {
        foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProp);
        }

        return query;
    }

    protected async Task<PaginatedResult<T>> Paginate(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}