namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public readonly record struct PurchasePatchModel(
  Maybe<Item> Item,
  Maybe<Category> Category,
  Maybe<Quantity> Quantity,
  Maybe<Money> UnitPrice,
  Maybe<Money> TotalDiscount,
  Maybe<Description> Description)
{
  public static PurchasePatchModel Create(
    Maybe<Item> item,
    Maybe<Category> category,
    Maybe<Quantity> quantity,
    Maybe<Money> unitPrice,
    Maybe<Money> totalDiscount,
    Maybe<Description> description)
    => new(item, category, quantity, unitPrice, totalDiscount, description);
}
