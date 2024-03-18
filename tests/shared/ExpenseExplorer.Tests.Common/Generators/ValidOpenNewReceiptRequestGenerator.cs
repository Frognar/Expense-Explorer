namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class ValidOpenNewReceiptRequestGenerator
{
  public static Arbitrary<OpenNewReceiptRequest> ValidOpenNewReceiptRequestGen()
  {
    return OpenNewReceiptRequestGenerator.Valid.Arbitrary;
  }
}
