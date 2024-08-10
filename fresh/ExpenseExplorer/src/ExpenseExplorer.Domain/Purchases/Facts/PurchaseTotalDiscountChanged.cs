using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public sealed record PurchaseTotalDiscountChanged(
  string PurchaseId,
  decimal Quantity,
  string Currency)
  : Fact
{
  public static PurchaseTotalDiscountChanged Create(
    PurchaseIdType purchaseId,
    MoneyType totalDiscount)
    => new(purchaseId.Value, totalDiscount.Amount, totalDiscount.Currency);
}
