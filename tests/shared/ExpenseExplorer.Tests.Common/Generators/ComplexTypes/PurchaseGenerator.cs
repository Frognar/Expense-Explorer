namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.ValueObjects;

public static class PurchaseGenerator
{
  public static Gen<Purchase> Gen()
    =>
      from item in ItemGenerator.Gen()
      from category in CategoryGenerator.Gen()
      from quantity in QuantityGenerator.Gen()
      from unitPrice in MoneyGenerator.Gen()
      from totalDiscount in MoneyGenerator.Gen()
      from description in DescriptionGenerator.Gen()
      select new Purchase(Id.Unique(), item, category, quantity, unitPrice, totalDiscount, description);

  public static Arbitrary<Purchase> Arbitrary() => Gen().ToArbitrary();
}
