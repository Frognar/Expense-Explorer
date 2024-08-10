using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public sealed record PurchaseCreated(
  string PurchaseId,
  string ReceiptId,
  string Item,
  string CategoryId,
  decimal Quantity,
  decimal UnitPriceAmount,
  string UnitPriceCurrency,
  decimal TotalDiscountAmount,
  string TotalDiscountCurrency,
  string Description) : Fact
{
  public static PurchaseCreated Create(
    PurchaseIdType purchaseId,
    ReceiptIdType receiptId,
    ItemType item,
    ExpenseCategoryIdType categoryId,
    QuantityType quantity,
    MoneyType unitPrice,
    MoneyType totalDiscount,
    DescriptionType description)
  {
    return new PurchaseCreated(
      purchaseId.Value,
      receiptId.Value,
      item.Value,
      categoryId.Value,
      quantity.Value,
      unitPrice.Amount,
      unitPrice.Currency,
      totalDiscount.Amount,
      totalDiscount.Currency,
      description.Value);
  }
}
