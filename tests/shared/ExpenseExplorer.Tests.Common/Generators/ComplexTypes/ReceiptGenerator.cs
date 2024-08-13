using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class ReceiptGenerator
{
  public static Gen<Receipt> Gen()
    =>
      from store in StoreGenerator.Gen()
      from purchaseDate in NonFutureDateGenerator.Gen()
      select Receipt.New(store, purchaseDate, purchaseDate.Date).ClearChanges();

  public static Arbitrary<Receipt> Arbitrary() => Gen().ToArbitrary();
}
