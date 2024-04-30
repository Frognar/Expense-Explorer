namespace ExpenseExplorer.Tests.Common.Generators.Commands;

using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public static class InvalidUpdateReceiptCommandGenerator
{
  public static Gen<UpdateReceiptCommand> Gen()
  {
    Gen<UpdateReceiptCommand> invalidStore =
      from store in ArbMap.Default.GeneratorFor<string>()
      where store is not null && store.Length > 0 && store.Trim().Length == 0
      from purchaseDate in ArbMap.Default.GeneratorFor<DateTime?>()
        .Select(dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : (DateOnly?)null)
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, purchaseDate ?? DateOnly.MaxValue);

    Gen<UpdateReceiptCommand> invalidPurchaseDate =
      from store in ArbMap.Default.GeneratorFor<NonWhiteSpaceString?>().Select(str => str?.Item)
      from purchaseDate in DateOnlyGenerator.Gen()
      select new UpdateReceiptCommand("receiptId", store, purchaseDate, purchaseDate.AddDays(-1));

    return FsCheck.Fluent.Gen.OneOf(invalidStore, invalidPurchaseDate);
  }

  public static Arbitrary<UpdateReceiptCommand> Arbitrary() => Gen().ToArbitrary();
}
