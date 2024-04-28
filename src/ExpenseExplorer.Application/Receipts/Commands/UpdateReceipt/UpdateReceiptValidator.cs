namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;
using FunctionalCore.Validations;

internal static class UpdateReceiptValidator
{
  public static Result<ReceiptPatchModel> Validate(UpdateReceiptCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Maybe<Store>, Maybe<PurchaseDate>, DateOnly, ReceiptPatchModel> createPatchModel
      = ReceiptPatchModel.Create;

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
        store => Validation.Succeeded(Some.From(store)));
  }

  private static Validated<Maybe<PurchaseDate>> Validate(DateOnly? date, DateOnly today)
  {
    if (!date.HasValue)
    {
      return Validation.Succeeded(None.OfType<PurchaseDate>());
    }

    return PurchaseDate.TryCreate(date.Value, today)
      .Match(
        () => Validation.Failed<Maybe<PurchaseDate>>(CommonFailures.FutureDate),
        purchaseDate => Validation.Succeeded(Some.From(purchaseDate)));
  }
}
