using ExpenseExplorer.API.Contract;

namespace ExpenseExplorer.API.Tests;

public class ReceiptAddingTests {
  [Property(Arbitrary = [typeof(MyGenerators)])]
  public void ContainsDataGivenDuringConstruction(string storeName, DateOnly purchaseDate) {
    OpenNewReceiptRequest request = new(storeName, purchaseDate);
    request.StoreName.Should().Be(storeName);
    request.PurchaseDate.Should().Be(purchaseDate);
  }
}
