namespace ExpenseExplorer.Application.Receipts;

using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public static class ReceiptValidator
{
  public static Validated<Receipt> Validate(string storeName, DateOnly purchaseDate, DateOnly today)
  {
    Func<Store, PurchaseDate, Receipt> createReceipt = Receipt.New;
    return createReceipt
      .Apply(Validate(storeName))
      .Apply(Validate(purchaseDate, today));
  }

  private static Validated<Store> Validate(string storeName)
  {
    return string.IsNullOrWhiteSpace(storeName)
      ? Validation.Failed<Store>([ValidationError.Create("StoreName", "EMPTY_STORE_NAME")])
      : Validation.Succeeded(Store.Create(storeName));
  }

  private static Validated<PurchaseDate> Validate(DateOnly purchaseDate, DateOnly today)
  {
    return purchaseDate > today
      ? Validation.Failed<PurchaseDate>([ValidationError.Create("PurchaseDate", "FUTURE_DATE")])
      : Validation.Succeeded(PurchaseDate.Create(purchaseDate, today));
  }
}
