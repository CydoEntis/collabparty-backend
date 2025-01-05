﻿using System.Linq.Expressions;

namespace CollabParty.Application.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeProperties = null);

    Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null);
    Task<T> CreateAsync(T entity);
    Task RemoveAsync(T entity);
    Task SaveAsync();

    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
}