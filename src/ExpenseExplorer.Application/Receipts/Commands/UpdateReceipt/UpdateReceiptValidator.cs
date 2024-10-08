using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Validations;

namespace ExpenseExplorer.Application.Receipts.Commands;

internal static class UpdateReceiptValidator
{
  public static Result<ReceiptPatchModel> Validate(UpdateReceiptCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Maybe<Store>, Maybe<NonFutureDate>, DateOnly, ReceiptPatchModel> createPatchModel = ReceiptPatchModel.Create;

    return createPatchModel
      .Apply(Validate(command.StoreName))
      .Apply(Validate(command.PurchaseDate, command.Today))
      .Apply(Validation.Succeeded(command.Today))
      .ToResult();
  }

  private static Validated<Maybe<Store>> Validate(string? storeName)
  {
    if (string.IsNullOrEmpty(storeName))
    {
      return Validation.Succeeded(None.OfType<Store>());
    }

    return Store.TryCreate(storeName)
      .Match(
        () => Validation.Failed<Maybe<Store>>(CommonFailures.EmptyStoreName),
        store => Validation.Succeeded(Some.With(store)));
  }

  private static Validated<Maybe<NonFutureDate>> Validate(DateOnly? date, DateOnly today)
  {
    if (!date.HasValue)
    {
      return Validation.Succeeded(None.OfType<NonFutureDate>());
    }

    return NonFutureDate.TryCreate(date.Value, today)
      .Match(
        () => Validation.Failed<Maybe<NonFutureDate>>(CommonFailures.FuturePurchaseDate),
        purchaseDate => Validation.Succeeded(Some.With(purchaseDate)));
  }
}
