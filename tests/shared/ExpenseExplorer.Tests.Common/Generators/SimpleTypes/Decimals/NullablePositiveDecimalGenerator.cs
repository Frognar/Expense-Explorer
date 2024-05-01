namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;

public static class NullablePositiveDecimalGenerator
{
  public static Gen<decimal?> Gen()
    => FsCheck.Fluent.Gen.OneOf(
      PositiveDecimalGenerator.Gen().Select(value => (decimal?)value),
      FsCheck.Fluent.Gen.Constant<decimal?>(null));

  public static Arbitrary<decimal?> Arbitrary() => Gen().ToArbitrary();
}
