namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidDescriptionGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidDescriptionGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      Description = ArbMap.Default.ArbFor<string?>().Filter(s => s != null && s.Trim().Length == 0).Generator,
    }).Arbitrary;
  }
}
