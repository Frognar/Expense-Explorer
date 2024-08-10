using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategories.Facts;

public sealed record ExpenseCategoryDeleted(
  string ExpenseCategoryId)
  : Fact
{
  public static ExpenseCategoryDeleted Create(
    ExpenseCategoryIdType expenseCategoryId)
    => new(expenseCategoryId.Value);
}
