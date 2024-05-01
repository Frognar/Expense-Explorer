namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public static class FutureDateOnlyGenerator
{
  public static Gen<DateOnly> Gen()
    =>
      from daysToAdd in FsCheck.Fluent.Gen.Choose(1, 365)
      select Today.AddDays(daysToAdd);

  public static Arbitrary<DateOnly> Arbitrary() => Gen().ToArbitrary();
}