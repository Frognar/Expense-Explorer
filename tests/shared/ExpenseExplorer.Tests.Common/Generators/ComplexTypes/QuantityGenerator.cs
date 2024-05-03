namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class QuantityGenerator
{
  public static Gen<Maybe<Quantity>> GenMaybe()
    =>
      from value in PositiveDecimalGenerator.Gen()
      select Quantity.TryCreate(value);

  public static Gen<Quantity> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Quantity> Arbitrary() => Gen().ToArbitrary();
}
