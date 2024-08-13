using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.ReadModel.Extensions;

public static class Page
{
  public static PageOf<T> Of<T>(IEnumerable<T> items, int totalCount, int filteredCount, int pageSize, int pageNumber)
    => new(items, totalCount, filteredCount, pageSize, pageNumber);
}
