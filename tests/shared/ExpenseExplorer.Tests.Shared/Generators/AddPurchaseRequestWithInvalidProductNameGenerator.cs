namespace ExpenseExplorer.Tests.Shared.Generators;

using API.Contract;

public class AddPurchaseRequestWithInvalidProductNameGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidProductNameGen()
  {
    return (AddPurchaseRequestGenerators.valid with
    {
      ProductName = ArbMap.Default.ArbFor<string>().Filter(string.IsNullOrWhiteSpace).Generator
    }).Arbitrary;
  }
}
