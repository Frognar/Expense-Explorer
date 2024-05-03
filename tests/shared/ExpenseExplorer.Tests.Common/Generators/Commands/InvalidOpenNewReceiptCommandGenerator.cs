namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidOpenNewReceiptCommandGenerator
{
  public static Arbitrary<OpenNewReceiptCommand> InvalidOpenNewReceiptCommandGen()
  {
    Gen<OpenNewReceiptCommand> invalidStoreName =
      from storeName in EmptyOrWhiteSpaceStringGenerator.Gen()
      from purchaseDate in DateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, purchaseDate);

    Gen<OpenNewReceiptCommand> invalidPurchaseDate =
      from storeName in NonEmptyStringGenerator.Gen()
      from purchaseDate in DateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate.AddDays(1), purchaseDate);

    Gen<OpenNewReceiptCommand> invalidStoreNameAndPurchaseDate =
      from storeName in EmptyOrWhiteSpaceStringGenerator.Gen()
      from purchaseDate in DateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate.AddDays(1), purchaseDate);

    return FsCheck.Fluent.Gen.OneOf(
        invalidStoreName,
        invalidPurchaseDate,
        invalidStoreNameAndPurchaseDate)
      .ToArbitrary();
  }
}
