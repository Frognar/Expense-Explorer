namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public static class NullableDateOnlyGenerator
{
  public static Gen<DateOnly?> Gen()
    => FsCheck.Fluent.Gen.OneOf(
      DateOnlyGenerator.Gen().Select(date => (DateOnly?)date),
      FsCheck.Fluent.Gen.Constant<DateOnly?>(null));

  public static Arbitrary<DateOnly?> Arbitrary() => Gen().ToArbitrary();
}
