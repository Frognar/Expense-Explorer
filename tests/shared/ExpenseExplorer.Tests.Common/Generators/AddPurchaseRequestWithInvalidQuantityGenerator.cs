namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class AddPurchaseRequestWithInvalidQuantityGenerator
{
  public static Arbitrary<AddPurchaseRequest> AddPurchaseRequestWithInvalidQuantityGen()
  {
    return (AddPurchaseRequestGenerators.Valid with
    {
      Quantity = NonPositiveDecimalGenerator.NonPositiveGen().Generator,
    }).Arbitrary;
  }
}
