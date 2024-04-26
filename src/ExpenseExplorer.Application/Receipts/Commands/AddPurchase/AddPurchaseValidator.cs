namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
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
      .Apply(Validation.Succeeded(Description.Create(command.Description)))
      .ToResult();
  }

  private static Validated<Item> ValidateItem(string item)
  {
    return Item.TryCreate(item)
      .Match(
        () => Validation.Failed<Item>(ValidationFailure.SingleFailure("Item", "EMPTY_ITEM_NAME")),
        Validation.Succeeded);
  }

  private static Validated<Category> ValidateCategory(string category)
  {
    return Category.TryCreate(category)
      .Match(
        () => Validation.Failed<Category>(ValidationFailure.SingleFailure("Category", "EMPTY_CATEGORY")),
        Validation.Succeeded);
  }

  private static Validated<Quantity> ValidateQuantity(decimal quantity)
  {
    return Quantity.TryCreate(quantity)
      .Match(
        () => Validation.Failed<Quantity>(ValidationFailure.SingleFailure("Quantity", "NON_POSITIVE_QUANTITY")),
        Validation.Succeeded);
  }

  private static Validated<Money> ValidateUnitPrice(decimal unitPrice)
  {
    return Money.TryCreate(unitPrice)
      .Match(
        () => Validation.Failed<Money>(ValidationFailure.SingleFailure("UnitPrice", "NEGATIVE_UNIT_PRICE")),
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
        () => Validation.Failed<Money>(ValidationFailure.SingleFailure("TotalDiscount", "NEGATIVE_TOTAL_DISCOUNT")),
        Validation.Succeeded);
  }
}
