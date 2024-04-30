namespace ExpenseExplorer.Tests.Common.Generators.Commands;

using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class InvalidOpenNewReceiptCommandGenerator
{
  public static Arbitrary<OpenNewReceiptCommand> InvalidOpenNewReceiptCommandGen()
  {
    Gen<OpenNewReceiptCommand> invalidStoreName =
      from storeName in EmptyStringGenerator.Gen()
      from purchaseDate in NonFutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

    Gen<OpenNewReceiptCommand> invalidPurchaseDate =
      from storeName in NonEmptyStringGenerator.Gen()
      from purchaseDate in FutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

    Gen<OpenNewReceiptCommand> invalidStoreNameAndPurchaseDate =
      from storeName in EmptyStringGenerator.Gen()
      from purchaseDate in FutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

    return FsCheck.Fluent.Gen.OneOf(
        invalidStoreName,
        invalidPurchaseDate,
        invalidStoreNameAndPurchaseDate)
      .ToArbitrary();
  }
}
