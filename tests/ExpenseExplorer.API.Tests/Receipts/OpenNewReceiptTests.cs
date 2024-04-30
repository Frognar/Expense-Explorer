namespace ExpenseExplorer.API.Tests.Receipts;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

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
