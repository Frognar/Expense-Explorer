namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidItemGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidItemGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      Item = ArbMap.Default.ArbFor<string>().Filter(string.IsNullOrWhiteSpace).Generator,
    }).Arbitrary;
  }
}
