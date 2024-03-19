namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.API.Contract;

public static class ValidOpenNewReceiptRequestGenerator
{
  public static Arbitrary<OpenNewReceiptRequest> ValidOpenNewReceiptRequestGen()
  {
    return (
        from storeName in NonEmptyStringGenerator.NonEmptyStringGen().Generator
        from purchaseDate in NonFutureDateOnlyGenerator.NonFutureDateOnlyGen().Generator
        select new OpenNewReceiptRequest(storeName, purchaseDate))
      .ToArbitrary();
  }
}
