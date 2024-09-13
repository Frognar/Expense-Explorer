using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategories.Facts;

public sealed record ExpenseCategoryCreated(
  string ExpenseCategoryId,
  string ExpenseCategoryGroupId,
  string Name,
  string Description) : Fact
{
  public static ExpenseCategoryCreated Create(
    ExpenseCategoryIdType expenseCategoryId,
    ExpenseCategoryGroupIdType expenseCategoryGroupId,
    NameType name,
    DescriptionType description)
    => new(expenseCategoryId.Value, expenseCategoryGroupId.Value, name.Value, description.Value);
}
