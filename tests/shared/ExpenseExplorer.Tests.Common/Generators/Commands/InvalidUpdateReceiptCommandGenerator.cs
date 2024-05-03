namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidUpdateReceiptCommandGenerator
{
  public static Gen<UpdateReceiptCommand> Gen()
  {
    Gen<UpdateReceiptCommand> invalidStore =
      from store in WhiteSpaceStringGenerator.Gen()
      from purchaseDate in NullableNonFutureDateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, Today);

    Gen<UpdateReceiptCommand> invalidPurchaseDate =
      from store in NullableNonEmptyStringGenerator.Gen()
      from purchaseDate in FutureDateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, Today);

    return FsCheck.Fluent.Gen.OneOf(invalidStore, invalidPurchaseDate);
  }

  public static Arbitrary<UpdateReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
