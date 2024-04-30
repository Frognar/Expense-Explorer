namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public static class NullableNonFutureDateOnlyGenerator
{
  public static Gen<DateOnly?> Gen()
    => FsCheck.Fluent.Gen.OneOf(
      NonFutureDateOnlyGenerator.Gen().Select(date => (DateOnly?)date),
      FsCheck.Fluent.Gen.Constant<DateOnly?>(null));

  public static Arbitrary<DateOnly?> Arbitrary() => Gen().ToArbitrary();
}
