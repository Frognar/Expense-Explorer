namespace ExpenseExplorer.ReadModel.Models;

public class PageOf<T>(IEnumerable<T> items, int totalCount)
{
  public IEnumerable<T> Items { get; } = items;

  public int TotalCount { get; } = totalCount;
}
