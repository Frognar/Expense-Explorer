using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public record PurchaseDeleted(string PurchaseId) : Fact
{
  public static PurchaseDeleted Create(PurchaseIdType purchaseId) => new(purchaseId.Value);
}
