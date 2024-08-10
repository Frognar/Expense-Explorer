namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ExpenseCategoryGroupIdType(string Value);

public static class ExpenseCategoryGroupId
{
  public static ExpenseCategoryGroupIdType Unique() => new(Id.Unique());
}
