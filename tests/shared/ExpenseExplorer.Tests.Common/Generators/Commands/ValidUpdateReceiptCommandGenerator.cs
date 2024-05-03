namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidUpdateReceiptCommandGenerator
{
  public static Gen<UpdateReceiptCommand> Gen()
    =>
      from store in NullableNonEmptyStringGenerator.Gen()
      from purchaseDate in NullableDateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, DateOnly.MaxValue);

  public static Arbitrary<UpdateReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
