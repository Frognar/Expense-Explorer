namespace ExpenseExplorer.Tests.Common.Generators.Commands;

using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class ValidOpenNewReceiptCommandGenerator
{
  public static Gen<OpenNewReceiptCommand> Gen()
    =>
      from storeName in NonEmptyStringGenerator.Gen()
      from purchaseDate in NonFutureDateOnlyGenerator.Gen()
      select new OpenNewReceiptCommand(storeName, purchaseDate, TodayDateOnly);

  public static Arbitrary<OpenNewReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
