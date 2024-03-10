namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidTotalDiscountGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidTotalDiscountGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      TotalDiscount = ArbMap.Default.ArbFor<decimal?>().Filter(d => d is < 0).Generator,
    }).Arbitrary;
  }
}
