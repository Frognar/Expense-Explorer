using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ReceiptIdType(string Value);

public static class ReceiptId
{
  public static ReceiptIdType Unique() => new(Id.Unique());

  public static Maybe<ReceiptIdType> Create(string value)
    => from id in Id.Create(value) select new ReceiptIdType(id);
}
