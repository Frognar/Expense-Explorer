using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public sealed record PurchaseUnitPriceChanged(
  string PurchaseId,
  decimal Amount,
  string Currency)
  : Fact
{
  public static PurchaseUnitPriceChanged Create(
    PurchaseIdType purchaseId,
    MoneyType unitPrice)
    => new(purchaseId.Value, unitPrice.Amount, unitPrice.Currency);
}
