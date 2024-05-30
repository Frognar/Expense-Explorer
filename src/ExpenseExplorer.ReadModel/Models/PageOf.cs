namespace ExpenseExplorer.ReadModel.Models;

public sealed class PageOf<T>(IEnumerable<T> items, int totalCount, int filteredCount, int pageSize, int pageNumber)
{
  public IEnumerable<T> Items { get; } = items;

  public int TotalCount { get; } = totalCount;

  public int FilteredCount { get; } = filteredCount;

  public int PageSize { get; } = pageSize;

  public int PageNumber { get; } = pageNumber;

  public int PageCount { get; } = (int)Math.Ceiling((double)totalCount / pageSize);
}
