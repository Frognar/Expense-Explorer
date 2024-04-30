namespace ExpenseExplorer.Tests.Common.Generators.Commands;

using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class ValidUpdateReceiptCommandGenerator
{
  public static Gen<UpdateReceiptCommand> Gen()
    =>
      from store in NullableNonEmptyStringGenerator.Gen()
      from purchaseDate in NullableNonFutureDateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, TodayDateOnly);

  public static Arbitrary<UpdateReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
