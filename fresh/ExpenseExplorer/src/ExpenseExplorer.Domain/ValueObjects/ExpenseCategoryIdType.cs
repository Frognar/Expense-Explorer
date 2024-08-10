namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ExpenseCategoryIdType(string Value);

public static class ExpenseCategoryId
{
  public static ExpenseCategoryIdType Unique() => new(Id.Unique());
}
