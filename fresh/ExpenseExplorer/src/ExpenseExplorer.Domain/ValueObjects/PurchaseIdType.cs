using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct PurchaseIdType(string Value);

public static class PurchaseId
{
  public static PurchaseIdType Unique() => new(Id.Unique());

  public static Maybe<PurchaseIdType> Create(string value)
    => from id in Id.Create(value) select new PurchaseIdType(id);
}
