namespace ExpenseExplorer.Domain.ValueObjects;

public record Purchase(
  Item Item,
  Category Category,
  Quantity Quantity,
  Money UnitPrice,
  Money TotalDiscount,
  Description Description)
{
  public static Purchase Create(
    Item item,
    Category category,
    Quantity quantity,
    Money unitPrice,
    Money totalDiscount,
    Description description)
  {
    return new Purchase(
      item,
      category,
      quantity,
      unitPrice,
      totalDiscount,
      description);
  }
}
