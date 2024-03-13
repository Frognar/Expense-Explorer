namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class PurchaseGenerator
{
  public static Arbitrary<Purchase> PurchaseGen()
  {
    return (
        from item in ItemGenerator.ItemGen().Generator
        from category in CategoryGenerator.CategoryGen().Generator
        from quantity in QuantityGenerator.QuantityGen().Generator
        from unitPrice in MoneyGenerator.MoneyGen().Generator
        from totalDiscount in MoneyGenerator.MoneyGen().Generator
        from description in DescriptionGenerator.DescriptionGen().Generator
        select new Purchase(item, category, quantity, unitPrice, totalDiscount, description))
      .ToArbitrary();
  }
}
