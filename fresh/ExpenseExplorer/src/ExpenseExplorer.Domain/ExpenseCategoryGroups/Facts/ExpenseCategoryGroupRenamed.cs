using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;

public sealed record ExpenseCategoryGroupRenamed(
  string ExpenseCategoryGroupId,
  string Name)
  : Fact
{
  public static ExpenseCategoryGroupRenamed Create(
    ExpenseCategoryGroupIdType expenseCategoryGroupId,
    NameType name)
    => new(expenseCategoryGroupId.Value, name.Value);
}
