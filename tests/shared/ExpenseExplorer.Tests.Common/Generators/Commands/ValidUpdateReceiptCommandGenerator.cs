namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidUpdateReceiptCommandGenerator
{
  public static Gen<UpdateReceiptCommand> Gen()
    =>
      from store in NullableNonEmptyStringGenerator.Gen()
      from purchaseDate in NullableNonFutureDateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, Today);

  public static Arbitrary<UpdateReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
