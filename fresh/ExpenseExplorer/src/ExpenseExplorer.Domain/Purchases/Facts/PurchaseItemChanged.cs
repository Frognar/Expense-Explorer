using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public record PurchaseItemChanged(string PurchaseId, string Item) : Fact
{
  public static PurchaseItemChanged Create(PurchaseIdType purchaseId, ItemType item) => new(purchaseId.Value, item.Value);
}
