namespace ExpenseExplorer.Application.Receipts;

using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Validations;

public static class ReceiptValidator
{
  public static Validated<Receipt> Validate(OpenNewReceiptCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Store, PurchaseDate, DateOnly, Receipt> createReceipt = Receipt.New;
    return createReceipt
      .Apply(Validate(command.StoreName))
      .Apply(Validate(command.PurchaseDate, command.Today))
      .Apply(Validation.Succeeded(command.Today));
  }

  private static Validated<Store> Validate(string storeName)
  {
    return string.IsNullOrWhiteSpace(storeName)
      ? Validation.Failed<Store>(ValidationFailure.SingleFailure("StoreName", "EMPTY_STORE_NAME"))
      : Validation.Succeeded(Store.Create(storeName));
  }

  private static Validated<PurchaseDate> Validate(DateOnly purchaseDate, DateOnly today)
  {
    return purchaseDate > today
      ? Validation.Failed<PurchaseDate>(ValidationFailure.SingleFailure("PurchaseDate", "FUTURE_DATE"))
      : Validation.Succeeded(PurchaseDate.Create(purchaseDate, today));
  }
}
