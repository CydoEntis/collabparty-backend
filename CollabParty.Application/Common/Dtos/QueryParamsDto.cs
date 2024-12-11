using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos;

public class QueryParamsDto
{
    public string? Search { get; set; }
    public string? OrderBy { get; set; } = Domain.Enums.OrderByOption.Desc;
    public string? SortBy { get; set; } = Domain.Enums.SortByOption.Name;
    public string? DateFilter { get; set; } = DateFilterOption.CreatedAt; 
    public PriorityLevelOption PriorityLevelOption { get; set; } 
    public int PageNumber { get; set; } = 1; 
    public int PageSize { get; set; } = 24; 
    public string? StartDate { get; set; } 
    public string? EndDate { get; set; } 
}