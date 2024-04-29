namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Application.Receipts.Commands;

public static class InvalidUpdateReceiptCommandGenerator
{
  public static Arbitrary<UpdateReceiptCommand> InvalidUpdateReceiptCommandGen()
  {
    var invalidStore =
      from store in ArbMap.Default.ArbFor<string>()
        .Filter(s => s is not null && s.Length > 0 && s.Trim().Length == 0)
        .Generator
      from purchaseDate in ArbMap.Default.ArbFor<DateTime?>()
        .Generator
        .Select(dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : (DateOnly?)null)
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, purchaseDate ?? DateOnly.MaxValue);

    var invalidPurchaseDate =
      from store in ArbMap.Default.ArbFor<NonWhiteSpaceString?>().Generator.Select(str => str?.Item)
      from purchaseDate in DateOnlyGenerator.DateOnlyGen().Generator
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, purchaseDate.AddDays(-1));

    return Gen.OneOf(invalidStore, invalidPurchaseDate).ToArbitrary();
  }
}
