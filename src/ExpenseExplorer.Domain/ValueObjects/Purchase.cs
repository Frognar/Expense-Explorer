namespace ExpenseExplorer.Domain.ValueObjects;

public record Purchase(
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
}
