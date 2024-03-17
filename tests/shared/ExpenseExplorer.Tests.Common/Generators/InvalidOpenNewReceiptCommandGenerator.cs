namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class InvalidOpenNewReceiptCommandGenerator
{
  public static Arbitrary<OpenNewReceiptCommand> InvalidOpenNewReceiptCommandGen()
  {
    var invalidStoreName =
      from storeName in EmptyStringGenerator.EmptyStringGen().Generator
      from purchaseDate in NonFutureDateOnlyGenerator.NonFutureDateOnlyGen().Generator
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

    var invalidPurchaseDate =
      from storeName in NonEmptyStringGenerator.NonEmptyStringGen().Generator
      from purchaseDate in FutureDateOnlyGenerator.FutureDateOnlyGen().Generator
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

    var invalidStoreNameAndPurchaseDate =
      from storeName in EmptyStringGenerator.EmptyStringGen().Generator
      from purchaseDate in FutureDateOnlyGenerator.FutureDateOnlyGen().Generator
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

    return Gen.OneOf(
        invalidStoreName,
        invalidPurchaseDate,
        invalidStoreNameAndPurchaseDate)
      .ToArbitrary();
  }
}
