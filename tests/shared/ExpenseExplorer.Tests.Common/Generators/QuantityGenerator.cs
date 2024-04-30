namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class QuantityGenerator
{
  public static Gen<Quantity> Gen()
    =>
      from value in PositiveDecimalGenerator.Gen()
      select Quantity.Create(value);

  public static Arbitrary<Quantity> Arbitrary() => Gen().ToArbitrary();
}
