namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class ValidOpenNewReceiptCommandGenerator
{
  public static Gen<OpenNewReceiptCommand> Gen()
    =>
      from storeName in NonEmptyStringGenerator.Gen()
      from purchaseDate in NonFutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

  public static Arbitrary<OpenNewReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
