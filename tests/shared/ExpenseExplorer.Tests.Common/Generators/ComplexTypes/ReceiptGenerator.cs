namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.Receipts;

public static class ReceiptGenerator
{
  public static Gen<Receipt> Gen()
    =>
      from store in StoreGenerator.Gen()
      from purchaseDate in PurchaseDateGenerator.Gen()
      select Receipt.New(store, purchaseDate, purchaseDate.Date).ClearChanges();

  public static Arbitrary<Receipt> Arbitrary() => Gen().ToArbitrary();
}
