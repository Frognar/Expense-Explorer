namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

internal static class UpdatePurchaseDetailsValidator
{
  public static Result<PurchasePatchModel> Validate(UpdatePurchaseDetailsCommand command)
  {
    Maybe<Item> item = string.IsNullOrWhiteSpace(command.Item) ? None.OfType<Item>() : Item.TryCreate(command.Item);
    Maybe<Category> category = string.IsNullOrWhiteSpace(command.Category)
      ? None.OfType<Category>()
      : Category.TryCreate(command.Category);

    Maybe<Quantity> quantity = command.Quantity.HasValue
      ? Quantity.TryCreate(command.Quantity.Value)
      : None.OfType<Quantity>();

    Maybe<Money> unitPrice
      = command.UnitPrice.HasValue ? Money.TryCreate(command.UnitPrice.Value) : None.OfType<Money>();

    Maybe<Money> totalDiscount = command.TotalDiscount.HasValue
      ? Money.TryCreate(command.TotalDiscount.Value)
      : None.OfType<Money>();

    Maybe<Description> description = string.IsNullOrWhiteSpace(command.Description)
      ? None.OfType<Description>()
      : Some.From(Description.Create(command.Description));

    return Success.From(
      PurchasePatchModel.Create(
        item,
        category,
        quantity,
        unitPrice,
        totalDiscount,
        description));
  }
}
