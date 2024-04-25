namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using FunctionalCore.Validations;

public static class UpdateReceiptValidator
{
  internal static Validated<UpdateReceiptPatchModel> Validate(UpdateReceiptCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Maybe<Store>, Maybe<PurchaseDate>, DateOnly, UpdateReceiptPatchModel> createPatchModel
      = UpdateReceiptPatchModel.Create;

    return createPatchModel
      .Apply(Validate(command.StoreName))
      .Apply(Validate(command.PurchaseDate, command.Today))
      .Apply(Validation.Succeeded(command.Today));
  }

  private static Validated<Maybe<Store>> Validate(string? storeName)
  {
    if (string.IsNullOrEmpty(storeName))
    {
      return Validation.Succeeded(None.OfType<Store>());
    }

    return Store.TryCreate(storeName)
      .Match(
        () => Validation.Failed<Maybe<Store>>(ValidationFailure.SingleFailure("StoreName", "EMPTY_STORE_NAME")),
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
        () => Validation.Failed<Maybe<PurchaseDate>>(ValidationFailure.SingleFailure("PurchaseDate", "FUTURE_DATE")),
        purchaseDate => Validation.Succeeded(Some.From(purchaseDate)));
  }
}
