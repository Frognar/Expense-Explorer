namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

internal readonly record struct PurchasePatchModel
{
  private PurchasePatchModel(
    Maybe<Item> item,
    Maybe<Category> category,
    Maybe<Quantity> quantity,
    Maybe<Money> unitPrice,
    Maybe<Money> totalDiscount,
    Maybe<Description> description)
  {
    Item = item;
    Category = category;
    Quantity = quantity;
    UnitPrice = unitPrice;
    TotalDiscount = totalDiscount;
    Description = description;
  }

  public Maybe<Item> Item { get; }

  public Maybe<Category> Category { get; }

  public Maybe<Quantity> Quantity { get; }

  public Maybe<Money> UnitPrice { get; }

  public Maybe<Money> TotalDiscount { get; }

  public Maybe<Description> Description { get; }

  public static PurchasePatchModel Create(
    Maybe<Item> item,
    Maybe<Category> category,
    Maybe<Quantity> quantity,
    Maybe<Money> unitPrice,
    Maybe<Money> totalDiscount,
    Maybe<Description> description)
    => new(item, category, quantity, unitPrice, totalDiscount, description);
}
