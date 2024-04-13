namespace ExpenseExplorer.ReadModel.Extensions;

using ExpenseExplorer.ReadModel.Models;

public static class Page
{
  public static PageOf<T> Of<T>(IEnumerable<T> items, int totalCount, int pageSize) => new(items, totalCount, pageSize);
}
