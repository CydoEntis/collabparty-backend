using CollabParty.Application.Common.Models;
using CollabParty.Application.Interfaces;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Questlog.Application.Common.Models;

namespace CollabParty.Infrastructure.Repositories;

public class QuestRepository : BaseRepository<Quest>, IQuestRepository
{
    private readonly AppDbContext _db;

    public QuestRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<Quest> UpdateAsync(Quest entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.Quests.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    
        private IQueryable<Quest> ApplySearch(IQueryable<Quest> query, string searchTerm)
    {
        return query.Where(p => p.Name.Contains(searchTerm));
    }

    public async Task<PaginatedResult<Quest>> GetPaginatedAsync(QueryParams<Quest>? queryParams)
    {
        // Base query
        IQueryable<Quest> query = _dbSet.AsNoTracking();

        if (queryParams != null)
        {
            if (queryParams.Filter != null)
                query = query.Where(queryParams.Filter);

            if (!string.IsNullOrEmpty(queryParams.Search))
                query = ApplySearch(query, queryParams.Search);

            if (!string.IsNullOrEmpty(queryParams.StartDate) || !string.IsNullOrEmpty(queryParams.EndDate))
                query = ApplyDateFilters(query, queryParams);

            if (!string.IsNullOrEmpty(queryParams.OrderBy) && !string.IsNullOrEmpty(queryParams.SortBy))
                query = ApplyOrderBy(query, queryParams.SortBy, queryParams.OrderBy);

            if (!string.IsNullOrEmpty(queryParams.IncludeProperties))
                query = ApplyIncludeProperties(query, queryParams.IncludeProperties);
        }

        if (queryParams == null || string.IsNullOrEmpty(queryParams.OrderBy) ||
            string.IsNullOrEmpty(queryParams.SortBy))
            query = query.OrderByDescending(p => p.CreatedAt);

        int pageNumber = queryParams?.PageNumber ?? 1;
        int pageSize = queryParams?.PageSize ?? 18;

        return await Paginate(query, pageNumber, pageSize);
    }
        
    private IQueryable<Quest> ApplyOrderBy(IQueryable<Quest> query, string filter,
        string sortDirection)
    {
        return filter switch
        {
            SortByOption.Name => sortDirection == OrderByOption.Asc
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name),
            SortByOption.CreatedAt => sortDirection == OrderByOption.Asc
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt),
            SortByOption.UpdatedAt => sortDirection == OrderByOption.Asc
                ? query.OrderBy(p => p.UpdatedAt)
                : query.OrderByDescending(p => p.UpdatedAt),
            _ => query
        };
    }

    private IQueryable<Quest> ApplyDateFilters(IQueryable<Quest> query,
        QueryParams<Quest> queryParams)
    {
        if (!string.IsNullOrEmpty(queryParams.StartDate) && DateTime.TryParse(queryParams.StartDate, out var startDate))
        {
            query = ApplyDateFilter(query, queryParams.DateFilter, startDate, ">=");
        }

        if (!string.IsNullOrEmpty(queryParams.EndDate) && DateTime.TryParse(queryParams.EndDate, out var endDate))
        {
            query = ApplyDateFilter(query, queryParams.DateFilter, endDate, "<=");
        }

        return query;
    }

    private IQueryable<Quest> ApplyDateFilter(IQueryable<Quest> query, string filterDate, DateTime date, string operatorSymbol)
    {
        var dateField = filterDate switch
        {
            SortByOption.CreatedAt => "CreatedAt",
            SortByOption.UpdatedAt => "UpdatedAt",
            _ => null
        };

        if (!string.IsNullOrEmpty(dateField))
        {
            if (operatorSymbol == ">=")
            {
                query = query.Where(p => EF.Property<DateTime>(p, dateField) >= date);
            }
            else if (operatorSymbol == "<=")
            {
                query = query.Where(p => EF.Property<DateTime>(p, dateField) <= date);
            }
        }

        return query;
    }
}