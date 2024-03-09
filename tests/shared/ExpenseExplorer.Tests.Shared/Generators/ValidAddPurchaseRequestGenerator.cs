namespace ExpenseExplorer.Tests.Shared.Generators;

using API.Contract;

public class ValidAddPurchaseRequestGenerator
{
  public static Arbitrary<AddPurchaseRequest> ValidAddPurchaseRequestGen()
  {
    return AddPurchaseRequestGenerators.valid.Arbitrary;
  }
}
