namespace ExpenseExplorer.API.Tests.Receipts;

using ExpenseExplorer.API.Contract;

public class AddPurchaseTests
{
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(DateOnlyGenerator), typeof(PositiveDecimalGenerator)])]
  public void ContainsDataGivenDuringConstruction(
    string productName,
    string productCategory,
    decimal quantity,
    decimal unitPrice,
    decimal? totalDiscount,
    string? description)
  {
    AddPurchaseRequest request = new(
      productName,
      productCategory,
      quantity,
      unitPrice,
      totalDiscount,
      description);

    request.ProductName.Should().Be(productName);
    request.ProductCategory.Should().Be(productCategory);
    request.Quantity.Should().Be(quantity);
    request.UnitPrice.Should().Be(unitPrice);
    request.TotalDiscount.Should().Be(totalDiscount);
    request.Description.Should().Be(description);
  }
}
