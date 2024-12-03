// using CollabParty.Application.Common.Models;
// using CollabParty.Application.Interfaces;
// using CollabParty.Domain.Entities;
// using CollabParty.Domain.Enums;
// using CollabParty.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore;
// using Questlog.Application.Common.Models;
//
// namespace CollabParty.Infrastructure.Repositories;
//
// public class UserPartyRepository : BaseRepository<UserParty>, IUserPartyRepository
// {
//     private readonly AppDbContext _db;
//
//     public UserPartyRepository(AppDbContext db) : base(db)
//     {
//         _db = db;
//     }
//
//
//     public async Task<PaginatedResult<UserParty>> GetPaginatedAsync(QueryParams<UserParty>? queryParams)
//     {
//         // Base query
//         IQueryable<UserParty> query = _dbSet.AsNoTracking();
//
//         if (queryParams != null)
//         {
//             if (queryParams.Filter != null)
//                 query = query.Where(queryParams.Filter);
//
//             if (!string.IsNullOrEmpty(queryParams.SearchTerm))
//                 query = ApplySearchTerm(query, queryParams.SearchTerm);
//
//             if (!string.IsNullOrEmpty(queryParams.StartDate) || !string.IsNullOrEmpty(queryParams.EndDate))
//                 query = ApplyDateFilters(query, queryParams);
//
//             if (!string.IsNullOrEmpty(queryParams.SortDirection) && !string.IsNullOrEmpty(queryParams.SortField))
//                 query = ApplySortDirection(query, queryParams.SortField, queryParams.SortDirection);
//
//             if (!string.IsNullOrEmpty(queryParams.IncludeProperties))
//                 query = ApplyIncludeProperties(query, queryParams.IncludeProperties);
//         }
//
//         if (queryParams == null || string.IsNullOrEmpty(queryParams.SortDirection) ||
//             string.IsNullOrEmpty(queryParams.SortField))
//             query = query.OrderByDescending(up => up.Party.CreatedAt);
//
//         int pageNumber = queryParams?.PageNumber ?? 1;
//         int pageSize = queryParams?.PageSize ?? 18;
//
//         return await Paginate(query, pageNumber, pageSize);
//     }
//
//
//     public async Task<List<UserParty>> GetMostRecentPartiesForUserAsync(
//         string userId,
//         string includeProperties = "")
//     {
//         IQueryable<UserParty> query = _db.UserParties
//             .AsNoTracking()
//             .Where(up => up.UserId == userId)
//             .OrderByDescending(up => up.Party.UpdatedAt)
//             .Take(5);
//
//         if (!string.IsNullOrEmpty(includeProperties))
//         {
//             foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
//             {
//                 query = query.Include(includeProperty.Trim());
//             }
//         }
//
//         return await query.ToListAsync();
//     }
//
//     public async Task<UserParty> UpdateAsync(UserParty entity)
//     {
//         entity.UpdatedAt = DateTime.Now;
//         _db.UserParties.Update(entity);
//         await _db.SaveChangesAsync();
//         return entity;
//     }
//
//     public async Task UpdateUsersAsync(List<UserParty> userParties)
//     {
//         if (userParties == null || !userParties.Any())
//             throw new ArgumentException("The userParties list cannot be null or empty.", nameof(userParties));
//
//         _db.UserParties.UpdateRange(userParties);
//         await _db.SaveChangesAsync();
//     }
//
//     public async Task RemoveUsersAsync(List<UserParty> userParties)
//     {
//         if (userParties == null || !userParties.Any())
//             throw new ArgumentException("The userParties list cannot be null or empty.", nameof(userParties));
//
//         _db.UserParties.RemoveRange(userParties);
//         await _db.SaveChangesAsync();
//     }
//
//     private IQueryable<UserParty> ApplySearchTerm(IQueryable<UserParty> query, string searchTerm)
//     {
//         return query.Where(up => up.Party.PartyName.Contains(searchTerm));
//     }
//
//     private IQueryable<UserParty> ApplySortDirection(IQueryable<UserParty> query, string filter, string sortDirection)
//     {
//         return filter switch
//         {
//             SortField.Name => sortDirection == SortDirection.Asc
//                 ? query.OrderBy(up => up.Party.PartyName)
//                 : query.OrderByDescending(up => up.Party.PartyName),
//             SortField.CreatedAt => sortDirection == SortDirection.Asc
//                 ? query.OrderBy(up => up.Party.CreatedAt)
//                 : query.OrderByDescending(up => up.Party.CreatedAt),
//             SortField.UpdatedAt => sortDirection == SortDirection.Asc
//                 ? query.OrderBy(up => up.Party.UpdatedAt)
//                 : query.OrderByDescending(up => up.Party.UpdatedAt),
//             _ => query
//         };
//     }
//
//     private IQueryable<UserParty> ApplyDateFilters(IQueryable<UserParty> query, QueryParams<UserParty> queryParams)
//     {
//         if (!string.IsNullOrEmpty(queryParams.StartDate) && DateTime.TryParse(queryParams.StartDate, out var startDate))
//         {
//             query = ApplyDateFilter(query, queryParams.DateFilterField, startDate, ">=");
//         }
//
//         if (!string.IsNullOrEmpty(queryParams.EndDate) && DateTime.TryParse(queryParams.EndDate, out var endDate))
//         {
//             query = ApplyDateFilter(query, queryParams.DateFilterField, endDate, "<=");
//         }
//
//         return query;
//     }
//
//     private IQueryable<UserParty> ApplyDateFilter(IQueryable<UserParty> query, string filterDate, DateTime date,
//         string operatorSymbol)
//     {
//         query = query.Include(up => up.Party);
//
//         var dateField = filterDate switch
//         {
//             SortField.CreatedAt => "CreatedAt",
//             SortField.UpdatedAt => "UpdatedAt",
//             _ => null
//         };
//
//         if (!string.IsNullOrEmpty(dateField))
//         {
//             if (operatorSymbol == ">=")
//             {
//                 query = query.Where(up => EF.Property<DateTime>(up.Party, dateField) >= date);
//             }
//             else if (operatorSymbol == "<=")
//             {
//                 query = query.Where(up => EF.Property<DateTime>(up.Party, dateField) <= date);
//             }
//         }
//
//         return query;
//     }
// }