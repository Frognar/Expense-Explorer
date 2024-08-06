namespace ExpenseExplorer.Domain.ValueObjects;

using DotMaybe;

public readonly record struct Purchase(
  Id Id,
  Item Item,
  Category Category,
  Quantity Quantity,
  Money UnitPrice,
  Money TotalDiscount,
  Description Description)
{
  public static Purchase Create(
    Id id,
    Item item,
    Category category,
    Quantity quantity,
    Money unitPrice,
    Money totalDiscount,
    Description description)
  {
    return new Purchase(
      id,
      item,
      category,
      quantity,
      unitPrice,
      totalDiscount,
      description);
  }

  public static Maybe<Purchase> TryCreate(
    Maybe<Id> maybeId,
    Maybe<Item> maybeItem,
    Maybe<Category> maybeCategory,
    Maybe<Quantity> maybeQuantity,
    Maybe<Money> maybeUnitPrice,
    Maybe<Money> maybeTotalDiscount,
    Maybe<Description> maybeDescription)
  {
    ArgumentNullException.ThrowIfNull(maybeId);
    return
      from id in maybeId
      from item in maybeItem
      from category in maybeCategory
      from quantity in maybeQuantity
      from unitPrice in maybeUnitPrice
      from totalDiscount in maybeTotalDiscount
      from description in maybeDescription
      select new Purchase(id, item, category, quantity, unitPrice, totalDiscount, description);
  }
}
