namespace ExpenseExplorer.API.Tests.Receipts;

public class OpenNewReceiptTests
{
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator), typeof(DateOnlyGenerator)])]
  public void ContainsDataGivenDuringConstruction(string storeName, DateOnly purchaseDate)
  {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);

    request.StoreName.Should().Be(storeName);
    request.PurchaseDate.Should().Be(purchaseDate);
  }
}
