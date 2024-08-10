using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;

public sealed record ExpenseCategoryGroupExpenseCategoryRemoved(
  string ExpenseCategoryGroupId,
  string ExpenseCategoryId)
  : Fact
{
  public static ExpenseCategoryGroupExpenseCategoryRemoved Create(
    ExpenseCategoryGroupIdType expenseCategoryGroupId,
    ExpenseCategoryIdType expenseCategoryId)
    => new(expenseCategoryGroupId.Value, expenseCategoryId.Value);
}
