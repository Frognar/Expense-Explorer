namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidProductCategoryGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidProductCategoryGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      ProductCategory = ArbMap.Default.ArbFor<string>().Filter(string.IsNullOrWhiteSpace).Generator,
    }).Arbitrary;
  }
}
