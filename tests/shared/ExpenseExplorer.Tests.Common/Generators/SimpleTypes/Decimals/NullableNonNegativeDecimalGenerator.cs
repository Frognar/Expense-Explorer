namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;

public static class NullableNonNegativeDecimalGenerator
{
  public static Gen<decimal?> Gen()
  {
    return FsCheck.Fluent.Gen.OneOf(
      NonNegativeDecimalGenerator.Gen().Select(d => (decimal?)d),
      FsCheck.Fluent.Gen.Constant<decimal?>(null));
  }

  public static Arbitrary<decimal?> Arbitrary() => Gen().ToArbitrary();
}
