using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategories.Facts;

public sealed record ExpenseCategoryDescriptionChanged(
  string ExpenseCategoryId,
  string Description)
  : Fact
{
  public static ExpenseCategoryDescriptionChanged Create(
    ExpenseCategoryIdType expenseCategoryId,
    DescriptionType description)
    => new(expenseCategoryId.Value, description.Value);
}
