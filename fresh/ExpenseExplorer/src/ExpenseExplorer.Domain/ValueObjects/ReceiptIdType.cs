namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ReceiptIdType(string Value);

public static class ReceiptId
{
  public static ReceiptIdType Unique() => new(Id.Unique());
}
