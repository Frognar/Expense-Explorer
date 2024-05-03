namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidUpdateReceiptCommandGenerator
{
  public static Gen<UpdateReceiptCommand> Gen()
  {
    Gen<UpdateReceiptCommand> invalidStore =
      from store in WhiteSpaceStringGenerator.Gen()
      from purchaseDate in NullableDateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, DateOnly.MaxValue);

    Gen<UpdateReceiptCommand> invalidPurchaseDate =
      from store in NullableNonEmptyStringGenerator.Gen()
      from purchaseDate in DateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate.AddDays(1), purchaseDate);

    return FsCheck.Fluent.Gen.OneOf(invalidStore, invalidPurchaseDate);
  }

  public static Arbitrary<UpdateReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
