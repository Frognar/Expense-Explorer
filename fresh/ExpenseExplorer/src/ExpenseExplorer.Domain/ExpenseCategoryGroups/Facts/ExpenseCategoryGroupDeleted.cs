using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;

public sealed record ExpenseCategoryGroupDeleted(string ExpenseCategoryGroupId)
  : Fact
{
  public static ExpenseCategoryGroupDeleted Create(
    ExpenseCategoryGroupIdType expenseCategoryGroupId)
    => new(expenseCategoryGroupId.Value);
}
