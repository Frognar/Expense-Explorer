using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;

public sealed record ExpenseCategoryGroupExpenseCategoryAdded(
  string ExpenseCategoryGroupId,
  string ExpenseCategoryId)
  : Fact
{
  public static ExpenseCategoryGroupExpenseCategoryAdded Create(
    ExpenseCategoryGroupIdType expenseCategoryGroupId,
    ExpenseCategoryIdType expenseCategoryId)
    => new(expenseCategoryGroupId.Value, expenseCategoryId.Value);
}
