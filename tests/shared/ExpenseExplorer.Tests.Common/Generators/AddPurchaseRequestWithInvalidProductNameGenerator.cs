namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidProductNameGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidProductNameGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      ProductName = ArbMap.Default.ArbFor<string>().Filter(string.IsNullOrWhiteSpace).Generator,
    }).Arbitrary;
  }
}
