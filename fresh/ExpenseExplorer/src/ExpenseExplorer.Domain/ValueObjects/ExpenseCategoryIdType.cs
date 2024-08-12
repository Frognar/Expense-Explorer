using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ExpenseCategoryIdType(string Value);

public static class ExpenseCategoryId
{
  public static ExpenseCategoryIdType Unique() => new(Id.Unique());

  public static Maybe<ExpenseCategoryIdType> Create(string value)
    => from id in Id.Create(value) select new ExpenseCategoryIdType(id);
}
