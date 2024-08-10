using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;

public sealed record ExpenseCategoryGroupDescriptionChanged(
  string ExpenseCategoryGroupId,
  string Description)
  : Fact
{
  public static ExpenseCategoryGroupDescriptionChanged Create(
    ExpenseCategoryGroupIdType expenseCategoryGroupId,
    DescriptionType description)
    => new(expenseCategoryGroupId.Value, description.Value);
}
