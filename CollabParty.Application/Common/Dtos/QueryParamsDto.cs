using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos;

public class QueryParamsDto
{
    public string? SearchTerm { get; set; }
    public string? SortDirection { get; set; } = Domain.Enums.SortDirection.Desc;
    public string? SortField { get; set; } = Domain.Enums.SortField.Title;
    public string? DateFilterField { get; set; } = DateFilter.CreatedAt; 
    public PriorityLevel PriorityLevel { get; set; } 
    public int PageNumber { get; set; } = 1; 
    public int PageSize { get; set; } = 18; 
    public string? StartDate { get; set; } 
    public string? EndDate { get; set; } 
}