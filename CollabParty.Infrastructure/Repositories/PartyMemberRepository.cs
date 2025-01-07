using CollabParty.Application.Common.Models;
using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Questlog.Application.Common.Models;

namespace CollabParty.Infrastructure.Repositories;

public class PartyMemberRepository : BaseRepository<PartyMember>, IPartyMemberRepository
{
    private readonly AppDbContext _db;

    public PartyMemberRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<PaginatedResult<PartyMember>> GetPaginatedAsync(QueryParams<PartyMember>? queryParams)
    {
        // Base query
        IQueryable<PartyMember> query = _dbSet.AsNoTracking();

        if (queryParams != null)
        {
            if (queryParams.Filter != null)
                query = query.Where(queryParams.Filter);

            if (!string.IsNullOrEmpty(queryParams.Search))
                query = Search(query, queryParams.Search);

            if (!string.IsNullOrEmpty(queryParams.StartDate) || !string.IsNullOrEmpty(queryParams.EndDate))
                query = ApplyDateFilters(query, queryParams);

            if (!string.IsNullOrEmpty(queryParams.OrderBy) && !string.IsNullOrEmpty(queryParams.SortBy))
                query = OrderBy(query, queryParams.SortBy, queryParams.OrderBy);

            if (!string.IsNullOrEmpty(queryParams.IncludeProperties))
                query = ApplyIncludeProperties(query, queryParams.IncludeProperties);
        }

        if (queryParams == null || string.IsNullOrEmpty(queryParams.OrderBy) ||
            string.IsNullOrEmpty(queryParams.SortBy))
            query = query.OrderByDescending(up => up.Party.CreatedAt);

        int pageNumber = queryParams?.PageNumber ?? 1;
        int pageSize = queryParams?.PageSize ?? 18;

        return await Paginate(query, pageNumber, pageSize);
    }


    public async Task<List<PartyMember>> GetMostRecentPartiesForUserAsync(
        string userId,
        string includeProperties = "")
    {
        IQueryable<PartyMember> query = _db.PartyMembers
            .AsNoTracking()
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.Party.UpdatedAt)
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

    public async Task<PartyMember> UpdateAsync(PartyMember entity)
    {
        entity.JoinedAt = DateTime.UtcNow;
        _db.PartyMembers.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateUsersAsync(List<PartyMember> userParties)
    {
        if (userParties == null || !userParties.Any())
            throw new ArgumentException("The userParties list cannot be null or empty.", nameof(userParties));

        _db.PartyMembers.UpdateRange(userParties);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveUsersAsync(List<PartyMember> userParties)
    {
        if (userParties == null || !userParties.Any())
            throw new ArgumentException("The userParties list cannot be null or empty.", nameof(userParties));

        _db.PartyMembers.RemoveRange(userParties);
        await _db.SaveChangesAsync();
    }

    private IQueryable<PartyMember> Search(IQueryable<PartyMember> query, string searchTerm)
    {
        return query.Where(up => up.Party.Name.Contains(searchTerm));
    }

    private IQueryable<PartyMember> OrderBy(IQueryable<PartyMember> query, string filter,
        string sortDirection)
    {
        return filter switch
        {
            SortByOption.Name => sortDirection == OrderByOption.Asc
                ? query.OrderBy(up => up.Party.Name)
                : query.OrderByDescending(up => up.Party.Name),
            SortByOption.CreatedAt => sortDirection == OrderByOption.Asc
                ? query.OrderBy(up => up.Party.CreatedAt)
                : query.OrderByDescending(up => up.Party.CreatedAt),
            SortByOption.UpdatedAt => sortDirection == OrderByOption.Asc
                ? query.OrderBy(up => up.Party.UpdatedAt)
                : query.OrderByDescending(up => up.Party.UpdatedAt),
            _ => query
        };
    }

    private IQueryable<PartyMember> ApplyDateFilters(IQueryable<PartyMember> query,
        QueryParams<PartyMember> queryParams)
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

    private IQueryable<PartyMember> ApplyDateFilter(IQueryable<PartyMember> query, string filterDate, DateTime date,
        string operatorSymbol)
    {
        query = query.Include(up => up.Party);

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
                query = query.Where(up => EF.Property<DateTime>(up.Party, dateField) >= date);
            }
            else if (operatorSymbol == "<=")
            {
                query = query.Where(up => EF.Property<DateTime>(up.Party, dateField) <= date);
            }
        }

        return query;
    }
}