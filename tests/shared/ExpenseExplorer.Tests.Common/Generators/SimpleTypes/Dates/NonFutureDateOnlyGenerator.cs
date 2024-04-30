namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public static class NonFutureDateOnlyGenerator
{
  public static Gen<DateOnly> Gen()
    =>
      from daysToAdd in FsCheck.Fluent.Gen.Choose(1, 365)
      select TodayDateOnly.AddDays(-daysToAdd);

  public static Arbitrary<DateOnly> Arbitrary() => Gen().ToArbitrary();
}
