using System.Collections.ObjectModel;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct PurchaseIdsType(IReadOnlyCollection<PurchaseIdType> Ids);

public static class PurchaseIds
{
  public static PurchaseIdsType Empty() => new([]);

  public static PurchaseIdsType New(params PurchaseIdType[] ids) => new(ids);

  public static bool Contains(this PurchaseIdsType ids, PurchaseIdType id) => ids.Ids.Contains(id);

  public static PurchaseIdsType Append(this PurchaseIdsType ids, PurchaseIdType id)
    => ids.Ids.Contains(id)
      ? ids
      : new PurchaseIdsType(new ReadOnlyCollection<PurchaseIdType>(ids.Ids.Append(id).ToList()));

  public static PurchaseIdsType Without(this PurchaseIdsType ids, PurchaseIdType id)
    => new(new ReadOnlyCollection<PurchaseIdType>(ids.Ids.Where(i => i != id).ToList()));
}
