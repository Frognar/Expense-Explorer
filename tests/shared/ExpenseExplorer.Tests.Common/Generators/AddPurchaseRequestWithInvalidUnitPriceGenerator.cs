namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidUnitPriceGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidUnitPriceGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      UnitPrice = NegativeDecimalGenerator.NegativeDecimalGen().Generator,
    }).Arbitrary;
  }
}
