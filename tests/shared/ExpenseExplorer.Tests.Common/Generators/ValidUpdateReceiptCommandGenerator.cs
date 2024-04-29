namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class ValidUpdateReceiptCommandGenerator
{
  public static Arbitrary<UpdateReceiptCommand> ValidUpdateReceiptCommandGen()
  {
    return (
        from store in ArbMap.Default.ArbFor<NonWhiteSpaceString?>()
          .Generator
          .Select(str => str?.Item)
        from purchaseDate in ArbMap.Default.ArbFor<DateTime?>()
          .Generator
          .Select(dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : (DateOnly?)null)
        select new UpdateReceiptCommand("receiptId", store, purchaseDate, purchaseDate ?? DateOnly.MaxValue))
      .ToArbitrary();
  }
}
