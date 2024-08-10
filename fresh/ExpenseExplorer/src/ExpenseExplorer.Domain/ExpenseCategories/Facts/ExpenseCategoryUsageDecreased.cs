using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategories.Facts;

public sealed record ExpenseCategoryUsageDecreased(
  string ExpenseCategoryId)
  : Fact
{
  public static ExpenseCategoryUsageDecreased Create(
    ExpenseCategoryIdType expenseCategoryId)
    => new(expenseCategoryId.Value);
}
