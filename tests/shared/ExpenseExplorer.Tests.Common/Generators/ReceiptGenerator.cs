namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.Receipts;

public static class ReceiptGenerator
{
  public static Arbitrary<Receipt> ReceiptGen()
  {
    return (
        from store in StoreGenerator.StoreGen().Generator
        from purchaseDate in PurchaseDateGenerator.PurchaseDateGen().Generator
        select Receipt.New(store, purchaseDate).ClearChanges())
      .ToArbitrary();
  }
}
