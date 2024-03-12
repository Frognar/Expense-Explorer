namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class QuantityGenerator
{
  public static Arbitrary<Quantity> QuantityGen()
  {
    return PositiveDecimalGenerator.PositiveDecimalGen()
      .Generator
      .Select(Quantity.Create)
      .ToArbitrary();
  }
}
