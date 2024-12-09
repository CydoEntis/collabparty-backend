using CollabParty.Application.Common.Models;
using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Questlog.Application.Common.Models;

namespace CollabParty.Infrastructure.Repositories;

public class PartyRepository : BaseRepository<Party>, IPartyRepository
{
    private readonly AppDbContext _db;

    public PartyRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<PaginatedResult<Party>> GetPaginatedAsync(QueryParams<Party>? queryParams)
    {
        // Base query
        IQueryable<Party> query = _dbSet.AsNoTracking();

        if (queryParams != null)
        {
            if (queryParams.Filter != null)
                query = query.Where(queryParams.Filter);

            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                query = ApplySearchTerm(query, queryParams.SearchTerm);

            if (!string.IsNullOrEmpty(queryParams.StartDate) || !string.IsNullOrEmpty(queryParams.EndDate))
                query = ApplyDateFilters(query, queryParams);

            if (!string.IsNullOrEmpty(queryParams.SortDirection) && !string.IsNullOrEmpty(queryParams.SortField))
                query = ApplySortDirection(query, queryParams.SortField, queryParams.SortDirection);

            if (!string.IsNullOrEmpty(queryParams.IncludeProperties))
                query = ApplyIncludeProperties(query, queryParams.IncludeProperties);
        }

        if (queryParams == null || string.IsNullOrEmpty(queryParams.SortDirection) ||
            string.IsNullOrEmpty(queryParams.SortField))
            query = query.OrderByDescending(p => p.CreatedAt);

        int pageNumber = queryParams?.PageNumber ?? 1;
        int pageSize = queryParams?.PageSize ?? 18;

        return await Paginate(query, pageNumber, pageSize);
    }


    public async Task<List<Party>> GetMostRecentPartiesForUserAsync(
        string userId,
        string includeProperties = "")
    {
        IQueryable<Party> query = _db.Parties
            .AsNoTracking()
            .Where(p => p.PartyMembers.Any(pm => pm.UserId == userId))
            .OrderByDescending(p => p.UpdatedAt)
            .Take(5);

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
        }

        return await query.ToListAsync();
    }


    public async Task<Party> UpdateAsync(Party entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.Parties.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task RemoveUsersAsync(List<Party> userParties)
    {
        if (userParties == null || !userParties.Any())
            throw new ArgumentException("The userParties list cannot be null or empty.", nameof(userParties));

        _db.Parties.RemoveRange(userParties);
        await _db.SaveChangesAsync();
    }

    private IQueryable<Party> ApplySearchTerm(IQueryable<Party> query, string searchTerm)
    {
        return query.Where(p => p.Name.Contains(searchTerm));
    }

    private IQueryable<Party> ApplySortDirection(IQueryable<Party> query, string filter,
        string sortDirection)
    {
        return filter switch
        {
            SortField.Name => sortDirection == SortDirection.Asc
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name),
            SortField.CreatedAt => sortDirection == SortDirection.Asc
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt),
            SortField.UpdatedAt => sortDirection == SortDirection.Asc
                ? query.OrderBy(p => p.UpdatedAt)
                : query.OrderByDescending(p => p.UpdatedAt),
            _ => query
        };
    }

    private IQueryable<Party> ApplyDateFilters(IQueryable<Party> query,
        QueryParams<Party> queryParams)
    {
        if (!string.IsNullOrEmpty(queryParams.StartDate) && DateTime.TryParse(queryParams.StartDate, out var startDate))
        {
            query = ApplyDateFilter(query, queryParams.DateFilterField, startDate, ">=");
        }

        if (!string.IsNullOrEmpty(queryParams.EndDate) && DateTime.TryParse(queryParams.EndDate, out var endDate))
        {
            query = ApplyDateFilter(query, queryParams.DateFilterField, endDate, "<=");
        }

        return query;
    }

    private IQueryable<Party> ApplyDateFilter(IQueryable<Party> query, string filterDate, DateTime date,
        string operatorSymbol)
    {
        var dateField = filterDate switch
        {
            SortField.CreatedAt => "CreatedAt",
            SortField.UpdatedAt => "UpdatedAt",
            _ => null
        };

        if (!string.IsNullOrEmpty(dateField))
        {
            if (operatorSymbol == ">=")
            {
                query = query.Where(p => EF.Property<DateTime>(p.CreatedAt, dateField) >= date);
            }
            else if (operatorSymbol == "<=")
            {
                query = query.Where(p => EF.Property<DateTime>(p.UpdatedAt, dateField) <= date);
            }
        }

        return query;
    }
}