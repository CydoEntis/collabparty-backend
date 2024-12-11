using System.Linq.Expressions;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Models;

public class QueryParams<T>
{
    public string? Search { get; set; }
    public string? OrderBy { get; set; }
    public string? SortBy { get; set; }
    public string? DateFilter { get; set; }
    public PriorityLevelOption PriorityLevelOption { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 18;
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? IncludeProperties { get; set; }
    public Expression<Func<T, bool>> Filter { get; set; }
}