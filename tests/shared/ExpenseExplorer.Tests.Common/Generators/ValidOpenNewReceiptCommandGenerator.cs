namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class ValidOpenNewReceiptCommandGenerator
{
  public static Arbitrary<OpenNewReceiptCommand> ValidOpenNewReceiptCommandGen()
  {
    return (
        from storeName in NonEmptyStringGenerator.NonEmptyStringGen().Generator
        from purchaseDate in NonFutureDateOnlyGenerator.NonFutureDateOnlyGen().Generator
        select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly))
      .ToArbitrary();
  }
}
