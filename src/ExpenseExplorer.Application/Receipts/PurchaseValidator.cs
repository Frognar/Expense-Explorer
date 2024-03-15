namespace ExpenseExplorer.Application.Receipts;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.ValueObjects;

public static class PurchaseValidator
{
  public static Validated<Purchase> Validate(AddPurchaseCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Item, Category, Quantity, Money, Money, Description, Purchase> createPurchase = Purchase.Create;
    return createPurchase
      .Apply(ValidateItem(command.Item))
      .Apply(ValidateCategory(command.Category))
      .Apply(ValidateQuantity(command.Quantity))
      .Apply(ValidateUnitPrice(command.UnitPrice))
      .Apply(ValidateTotalDiscount(command.TotalDiscount))
      .Apply(Validation.Succeeded(Description.Create(command.Description)));
  }

  private static Validated<Item> ValidateItem(string item)
  {
    return string.IsNullOrWhiteSpace(item)
      ? Validation.Failed<Item>([ValidationError.Create("Item", "EMPTY_ITEM_NAME")])
      : Validation.Succeeded(Item.Create(item));
  }

  private static Validated<Category> ValidateCategory(string category)
  {
    return string.IsNullOrWhiteSpace(category)
      ? Validation.Failed<Category>([ValidationError.Create("Category", "EMPTY_CATEGORY")])
      : Validation.Succeeded(Category.Create(category));
  }

  private static Validated<Quantity> ValidateQuantity(decimal quantity)
  {
    return quantity <= 0
      ? Validation.Failed<Quantity>([ValidationError.Create("Quantity", "NON_POSITIVE_QUANTITY")])
      : Validation.Succeeded(Quantity.Create(quantity));
  }

  private static Validated<Money> ValidateUnitPrice(decimal unitPrice)
  {
    return unitPrice < 0
      ? Validation.Failed<Money>([ValidationError.Create("UnitPrice", "NEGATIVE_UNIT_PRICE")])
      : Validation.Succeeded(Money.Create(unitPrice));
  }

  private static Validated<Money> ValidateTotalDiscount(decimal? totalDiscount)
  {
    if (totalDiscount is < 0)
    {
      return Validation.Failed<Money>([ValidationError.Create("TotalDiscount", "NEGATIVE_TOTAL_DISCOUNT")]);
    }

    return totalDiscount.HasValue
      ? Validation.Succeeded(Money.Create(totalDiscount.Value))
      : Validation.Succeeded(Money.Zero);
  }
}
