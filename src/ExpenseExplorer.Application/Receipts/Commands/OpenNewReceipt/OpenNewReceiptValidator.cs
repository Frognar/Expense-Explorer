namespace ExpenseExplorer.Application.Receipts.Commands;

using DotResult;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Validations;

internal static class OpenNewReceiptValidator
{
  public static Result<Receipt> Validate(OpenNewReceiptCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Store, NonFutureDate, DateOnly, Receipt> createReceipt = Receipt.New;
    return createReceipt
      .Apply(Validate(command.StoreName))
      .Apply(Validate(command.PurchaseDate, command.Today))
      .Apply(Validation.Succeeded(command.Today))
      .ToResult();
  }

  private static Validated<Store> Validate(string storeName)
  {
    return Store.TryCreate(storeName)
      .Match(
        () => Validation.Failed<Store>(CommonFailures.EmptyStoreName),
        Validation.Succeeded);
  }

  private static Validated<NonFutureDate> Validate(DateOnly purchaseDate, DateOnly today)
  {
    return NonFutureDate.TryCreate(purchaseDate, today)
      .Match(
        () => Validation.Failed<NonFutureDate>(CommonFailures.FuturePurchaseDate),
        Validation.Succeeded);
  }
}
