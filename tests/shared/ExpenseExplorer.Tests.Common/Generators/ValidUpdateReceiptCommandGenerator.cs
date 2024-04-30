namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class ValidUpdateReceiptCommandGenerator
{
  public static Gen<UpdateReceiptCommand> Gen()
    =>
      from store in ArbMap.Default.GeneratorFor<NonWhiteSpaceString?>()
        .Select(str => str?.Item)
      from purchaseDate in ArbMap.Default.GeneratorFor<DateTime?>()
        .Select(dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : (DateOnly?)null)
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, purchaseDate ?? DateOnly.MaxValue);

  public static Arbitrary<UpdateReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
