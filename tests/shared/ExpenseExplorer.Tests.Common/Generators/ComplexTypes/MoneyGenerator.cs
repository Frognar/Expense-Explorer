namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class MoneyGenerator
{
  public static Gen<Maybe<Money>> GenMaybe()
    =>
      from value in NonNegativeDecimalGenerator.Gen()
      select Money.TryCreate(value);

  public static Gen<Money> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Money> Arbitrary() => Gen().ToArbitrary();
}
