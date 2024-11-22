namespace Questlog.Application.Common.Models;

public class PaginatedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public List<int> PageRange { get; set; }

    public PaginatedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalItems = totalCount;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = CurrentPage < TotalPages;
        HasPreviousPage = CurrentPage > 1;
        PageRange = GeneratePageRange(CurrentPage, TotalPages);
    }

    private List<int> GeneratePageRange(int currentPage, int totalPages)
    {
        var range = new List<int>();
        int start = Math.Max(1, currentPage - 1);
        int end = Math.Min(totalPages, currentPage + 3);
        for (int i = start; i <= end; i++)
        {
            range.Add(i);
        }

        return range;
    }
}