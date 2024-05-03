namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidOpenNewReceiptCommandGenerator
{
  public static Arbitrary<OpenNewReceiptCommand> InvalidOpenNewReceiptCommandGen()
  {
    Gen<OpenNewReceiptCommand> invalidStoreName =
      from storeName in EmptyOrWhiteSpaceStringGenerator.Gen()
      from purchaseDate in NonFutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, Today);

    Gen<OpenNewReceiptCommand> invalidPurchaseDate =
      from storeName in NonEmptyStringGenerator.Gen()
      from purchaseDate in FutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, Today);

    Gen<OpenNewReceiptCommand> invalidStoreNameAndPurchaseDate =
      from storeName in EmptyOrWhiteSpaceStringGenerator.Gen()
      from purchaseDate in FutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, Today);

    return FsCheck.Fluent.Gen.OneOf(
        invalidStoreName,
        invalidPurchaseDate,
        invalidStoreNameAndPurchaseDate)
      .ToArbitrary();
  }
}
