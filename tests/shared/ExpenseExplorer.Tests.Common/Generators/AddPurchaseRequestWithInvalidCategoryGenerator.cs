namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidCategoryGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidCategoryGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      Category = ArbMap.Default.ArbFor<string>().Filter(string.IsNullOrWhiteSpace).Generator,
    }).Arbitrary;
  }
}
