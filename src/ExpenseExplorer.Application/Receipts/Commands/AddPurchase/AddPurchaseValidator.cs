namespace ExpenseExplorer.Application.Receipts.Commands;

using DotResult;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Validations;

internal static class AddPurchaseValidator
{
  public static Result<Purchase> Validate(AddPurchaseCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Id, Item, Category, Quantity, Money, Money, Description, Purchase> createPurchase = Purchase.Create;
    return createPurchase
      .Apply(Validation.Succeeded(Id.Unique()))
      .Apply(ValidateItem(command.Item))
      .Apply(ValidateCategory(command.Category))
      .Apply(ValidateQuantity(command.Quantity))
      .Apply(ValidateUnitPrice(command.UnitPrice))
      .Apply(ValidateTotalDiscount(command.TotalDiscount))
      .Apply(ValidateDescription(command.Description))
      .ToResult();
  }

  private static Validated<Item> ValidateItem(string item)
  {
    return Item.TryCreate(item)
      .Match(
        () => Validation.Failed<Item>(CommonFailures.EmptyItemName),
        Validation.Succeeded);
  }

  private static Validated<Category> ValidateCategory(string category)
  {
    return Category.TryCreate(category)
      .Match(
        () => Validation.Failed<Category>(CommonFailures.EmptyCategory),
        Validation.Succeeded);
  }

  private static Validated<Quantity> ValidateQuantity(decimal quantity)
  {
    return Quantity.TryCreate(quantity)
      .Match(
        () => Validation.Failed<Quantity>(CommonFailures.NonPositiveQuantity),
        Validation.Succeeded);
  }

  private static Validated<Money> ValidateUnitPrice(decimal unitPrice)
  {
    return Money.TryCreate(unitPrice)
      .Match(
        () => Validation.Failed<Money>(CommonFailures.NegativeUnitPrice),
        Validation.Succeeded);
  }

  private static Validated<Money> ValidateTotalDiscount(decimal? totalDiscount)
  {
    if (!totalDiscount.HasValue)
    {
      return Validation.Succeeded(Money.Zero);
    }

    return Money.TryCreate(totalDiscount.Value)
      .Match(
        () => Validation.Failed<Money>(CommonFailures.NegativeTotalDiscount),
        Validation.Succeeded);
  }

  private static Validated<Description> ValidateDescription(string? description)
  {
    if (description is null)
    {
      return Validation.Succeeded(Description.Empty);
    }

    return Description.TryCreate(description)
      .Match(
        () => Validation.Failed<Description>(CommonFailures.InvalidDescription),
        Validation.Succeeded);
  }
}
