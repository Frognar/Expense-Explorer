namespace ExpenseExplorer.Domain.Tests;

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

  [Property(
    Arbitrary =
    [
      typeof(ItemGenerator),
      typeof(CategoryGenerator),
      typeof(QuantityGenerator),
      typeof(MoneyGenerator),
      typeof(DescriptionGenerator)
    ])]
  public void CanBeCreatedWithRecordSyntax(
    Item item,
    Category category,
    Quantity quantity,
    Money unitPrice,
    Money totalDiscount,
    Description description)
  {
    Purchase purchase = Purchase.Create(
        Id.Unique(),
        Item.Create("i"),
        Category.Create("c"),
        Quantity.Create(1),
        Money.Create(1),
        Money.Create(1),
        Description.Create("d"))
      with
      {
        Item = item,
        Category = category,
        Quantity = quantity,
        UnitPrice = unitPrice,
        TotalDiscount = totalDiscount,
        Description = description,
      };

    purchase.Item.Should().Be(item);
    purchase.Category.Should().Be(category);
    purchase.Quantity.Should().Be(quantity);
    purchase.UnitPrice.Should().Be(unitPrice);
    purchase.TotalDiscount.Should().Be(totalDiscount);
    purchase.Description.Should().Be(description);
  }
}
