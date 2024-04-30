namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;

public static class QuantityGenerator
{
  public static Gen<Quantity> Gen()
    =>
      from value in PositiveDecimalGenerator.Gen()
      select Quantity.Create(value);

  public static Arbitrary<Quantity> Arbitrary() => Gen().ToArbitrary();
}
