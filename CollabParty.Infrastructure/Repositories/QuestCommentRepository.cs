using CollabParty.Application.Common.Models;
using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Questlog.Application.Common.Models;

namespace CollabParty.Infrastructure.Repositories;

public class QuestCommentRepository : BaseRepository<QuestComment>, IQuestCommentRepository
{
    private readonly AppDbContext _db;

    public QuestCommentRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<QuestComment> UpdateAsync(QuestComment entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.QuestComments.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<PaginatedResult<QuestComment>> GetPaginatedAsync(QueryParams<QuestComment>? queryParams)
    {
        IQueryable<QuestComment> query = _dbSet.AsNoTracking();

        if (queryParams != null)
        {
            if (queryParams.Filter != null)
                query = query.Where(queryParams.Filter);

            if (!string.IsNullOrEmpty(queryParams.IncludeProperties))
                query = ApplyIncludeProperties(query, queryParams.IncludeProperties);
        }

        if (queryParams == null || string.IsNullOrEmpty(queryParams.OrderBy) ||
            string.IsNullOrEmpty(queryParams.SortBy))
            query = query.OrderByDescending(qc => qc.CreatedAt);

        int pageNumber = queryParams?.PageNumber ?? 1;
        int pageSize = queryParams?.PageSize ?? 18;

        return await Paginate(query, pageNumber, pageSize);
    }
}