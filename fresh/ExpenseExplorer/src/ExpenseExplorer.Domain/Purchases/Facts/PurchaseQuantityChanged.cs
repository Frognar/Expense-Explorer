using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public record PurchaseQuantityChanged(string PurchaseId, decimal Quantity) : Fact
{
  public static PurchaseQuantityChanged Create(PurchaseIdType purchaseId, QuantityType quantity)
    => new(purchaseId.Value, quantity.Value);
}
