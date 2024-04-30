namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public static class FutureDateOnlyGenerator
{
  public static Gen<DateOnly> Gen()
    =>
      from dateOnly in DateOnlyGenerator.Gen()
      where dateOnly > TodayDateOnly
      select dateOnly;

  public static Arbitrary<DateOnly> Arbitrary() => Gen().ToArbitrary();
}
