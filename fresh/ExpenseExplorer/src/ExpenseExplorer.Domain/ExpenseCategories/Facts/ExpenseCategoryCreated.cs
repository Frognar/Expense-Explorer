using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategories.Facts;

public sealed record ExpenseCategoryCreated(
  string ExpenseCategoryId,
  string Name,
  string Description) : Fact
{
  public static ExpenseCategoryCreated Create(
    ExpenseCategoryIdType expenseCategoryId,
    NameType name,
    DescriptionType description)
    => new(expenseCategoryId.Value, name.Value, description.Value);
}
