using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public sealed record PurchaseDeleted(string PurchaseId) : Fact
{
  public static PurchaseDeleted Create(PurchaseIdType purchaseId)
    => new(purchaseId.Value);
}
