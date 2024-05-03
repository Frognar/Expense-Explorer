namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class PurchaseGenerator
{
  public static Gen<Maybe<Purchase>> GenMaybe()
    =>
      from id in IdGenerator.GenMaybe()
      from item in ItemGenerator.GenMaybe()
      from category in CategoryGenerator.GenMaybe()
      from quantity in QuantityGenerator.GenMaybe()
      from unitPrice in MoneyGenerator.GenMaybe()
      from totalDiscount in MoneyGenerator.GenMaybe()
      from description in DescriptionGenerator.GenMaybe()
      select Purchase.TryCreate(id, item, category, quantity, unitPrice, totalDiscount, description);

  public static Gen<Purchase> Gen()
    =>
      from purchase in GenMaybe()
      select purchase.ForceValue();

  public static Arbitrary<Purchase> Arbitrary() => Gen().ToArbitrary();
}
