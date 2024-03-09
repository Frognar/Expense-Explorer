namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class ValidAddPurchaseRequestGenerator
{
  public static Arbitrary<AddPurchaseRequest> ValidAddPurchaseRequestGen()
  {
    return AddPurchaseRequestGenerators.Valid.Arbitrary;
  }
}
