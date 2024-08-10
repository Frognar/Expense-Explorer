using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategories.Facts;

public sealed record ExpenseCategoryRenamed(
  string ExpenseCategoryId,
  string Name)
  : Fact
{
  public static ExpenseCategoryRenamed Create(
    ExpenseCategoryIdType expenseCategoryId,
    NameType name)
    => new(expenseCategoryId.Value, name.Value);
}
