namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class PurchaseGenerator
{
  public static Gen<Purchase> Gen()
    =>
      from id in IdGenerator.Gen()
      from item in ItemGenerator.Gen()
      from category in CategoryGenerator.Gen()
      from quantity in QuantityGenerator.Gen()
      from unitPrice in MoneyGenerator.Gen()
      from totalDiscount in MoneyGenerator.Gen()
      from description in DescriptionGenerator.Gen()
      select Purchase.Create(id, item, category, quantity, unitPrice, totalDiscount, description);

  public static Arbitrary<Purchase> Arbitrary() => Gen().ToArbitrary();
}
