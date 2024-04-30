namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public class PurchaseTests
{
  [Property(
    Arbitrary =
    [
      typeof(ItemGenerator),
      typeof(CategoryGenerator),
      typeof(QuantityGenerator),
      typeof(MoneyGenerator),
      typeof(DescriptionGenerator)
    ])]
  public void CanBeCreated(
    Item item,
    Category category,
    Quantity quantity,
    Money unitPrice,
    Money totalDiscount,
    Description description)
  {
    Purchase purchase = Purchase.Create(Id.Unique(), item, category, quantity, unitPrice, totalDiscount, description);
    purchase.Item.Should().Be(item);
    purchase.Category.Should().Be(category);
    purchase.Quantity.Should().Be(quantity);
    purchase.UnitPrice.Should().Be(unitPrice);
    purchase.TotalDiscount.Should().Be(totalDiscount);
    purchase.Description.Should().Be(description);
  }
}
