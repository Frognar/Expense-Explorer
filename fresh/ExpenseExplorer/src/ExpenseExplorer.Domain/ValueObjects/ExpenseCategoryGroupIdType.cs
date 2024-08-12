using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ExpenseCategoryGroupIdType(string Value);

public static class ExpenseCategoryGroupId
{
  public static ExpenseCategoryGroupIdType Unique() => new(Id.Unique());

  public static Maybe<ExpenseCategoryGroupIdType> Create(string value)
    => from id in Id.Create(value) select new ExpenseCategoryGroupIdType(id);
}
