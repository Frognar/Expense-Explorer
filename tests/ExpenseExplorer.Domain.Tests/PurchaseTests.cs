namespace ExpenseExplorer.Domain.Tests;

public class PurchaseTests
{
  [Property(
    Arbitrary =
    [
      typeof(IdGenerator),
      typeof(ItemGenerator),
      typeof(CategoryGenerator),
      typeof(QuantityGenerator),
      typeof(MoneyGenerator),
      typeof(DescriptionGenerator)
    ])]
  public void CanBeCreated(
    Id id,
    Item item,
    Category category,
    Quantity quantity,
    Money unitPrice,
    Money totalDiscount,
    Description description)
  {
    Purchase purchase = Purchase.Create(id, item, category, quantity, unitPrice, totalDiscount, description);
    purchase.Id.Should().Be(id);
    purchase.Item.Should().Be(item);
    purchase.Category.Should().Be(category);
    purchase.Quantity.Should().Be(quantity);
    purchase.UnitPrice.Should().Be(unitPrice);
    purchase.TotalDiscount.Should().Be(totalDiscount);
    purchase.Description.Should().Be(description);
  }
}
