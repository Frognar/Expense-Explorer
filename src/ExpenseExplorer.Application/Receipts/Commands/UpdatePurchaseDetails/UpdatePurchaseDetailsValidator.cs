namespace ExpenseExplorer.Application.Receipts.Commands;

using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Validations;

internal static class UpdatePurchaseDetailsValidator
{
  public static Result<PurchasePatchModel> Validate(UpdatePurchaseDetailsCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Maybe<Item>, Maybe<Category>, Maybe<Quantity>, Maybe<Money>, Maybe<Money>, Maybe<Description>,
      PurchasePatchModel> createPatchModel = PurchasePatchModel.Create;

    return createPatchModel
      .Apply(ValidateItem(command.Item))
      .Apply(ValidateCategory(command.Category))
      .Apply(ValidateQuantity(command.Quantity))
      .Apply(ValidateUnitPrice(command.UnitPrice))
      .Apply(ValidateTotalDiscount(command.TotalDiscount))
      .Apply(ValidateDescription(command.Description))
      .ToResult();
  }

  private static Validated<Maybe<Item>> ValidateItem(string? item)
  {
    if (item is null)
    {
      return Validation.Succeeded(None.OfType<Item>());
    }

    return Item.TryCreate(item)
      .Match(
        () => Validation.Failed<Maybe<Item>>(CommonFailures.EmptyItemName),
        i => Validation.Succeeded(Some.With(i)));
  }

  private static Validated<Maybe<Category>> ValidateCategory(string? category)
  {
    if (category is null)
    {
      return Validation.Succeeded(None.OfType<Category>());
    }

    return Category.TryCreate(category)
      .Match(
        () => Validation.Failed<Maybe<Category>>(CommonFailures.EmptyCategory),
        c => Validation.Succeeded(Some.With(c)));
  }

  private static Validated<Maybe<Quantity>> ValidateQuantity(decimal? quantity)
  {
    if (!quantity.HasValue)
    {
      return Validation.Succeeded(None.OfType<Quantity>());
    }

    return Quantity.TryCreate(quantity.Value)
      .Match(
        () => Validation.Failed<Maybe<Quantity>>(CommonFailures.NonPositiveQuantity),
        q => Validation.Succeeded(Some.With(q)));
  }

  private static Validated<Maybe<Money>> ValidateUnitPrice(decimal? unitPrice)
  {
    if (!unitPrice.HasValue)
    {
      return Validation.Succeeded(None.OfType<Money>());
    }

    return Money.TryCreate(unitPrice.Value)
      .Match(
        () => Validation.Failed<Maybe<Money>>(CommonFailures.NegativeUnitPrice),
        up => Validation.Succeeded(Some.With(up)));
  }

  private static Validated<Maybe<Money>> ValidateTotalDiscount(decimal? totalDiscount)
  {
    if (!totalDiscount.HasValue)
    {
      return Validation.Succeeded(None.OfType<Money>());
    }

    return Money.TryCreate(totalDiscount.Value)
      .Match(
        () => Validation.Failed<Maybe<Money>>(CommonFailures.NegativeTotalDiscount),
        td => Validation.Succeeded(Some.With(td)));
  }

  private static Validated<Maybe<Description>> ValidateDescription(string? description)
  {
    if (description is null)
    {
      return Validation.Succeeded(None.OfType<Description>());
    }

    return Description.TryCreate(description)
      .Match(
        () => Validation.Failed<Maybe<Description>>(CommonFailures.InvalidDescription),
        d => Validation.Succeeded(Some.With(d)));
  }
}
