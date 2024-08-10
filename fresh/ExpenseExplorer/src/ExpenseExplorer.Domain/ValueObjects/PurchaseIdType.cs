namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct PurchaseIdType(string Value);

public static class PurchaseId
{
  public static PurchaseIdType Unique() => new(Id.Unique());
}
