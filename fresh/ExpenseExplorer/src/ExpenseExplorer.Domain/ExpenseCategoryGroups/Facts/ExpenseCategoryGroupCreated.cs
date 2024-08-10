using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;

public sealed record ExpenseCategoryGroupCreated(
  string ExpenseCategoryGroupId,
  string Name,
  string Description)
  : Fact
{
  public static ExpenseCategoryGroupCreated Create(
    ExpenseCategoryGroupIdType expenseCategoryGroupId,
    NameType name,
    DescriptionType description)
    => new(expenseCategoryGroupId.Value, name.Value, description.Value);
}
