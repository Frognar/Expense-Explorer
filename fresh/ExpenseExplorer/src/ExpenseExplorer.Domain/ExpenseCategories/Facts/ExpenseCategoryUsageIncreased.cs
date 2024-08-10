using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategories.Facts;

public sealed record ExpenseCategoryUsageIncreased(
  string ExpenseCategoryId)
  : Fact
{
  public static ExpenseCategoryUsageIncreased Create(
    ExpenseCategoryIdType expenseCategoryId)
    => new(expenseCategoryId.Value);
}
