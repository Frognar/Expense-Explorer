namespace ExpenseExplorer.Tests.Common.Generators;

public static class DateOnlyGenerator
{
  public static Gen<DateOnly> Gen()
    =>
      from dateTime in ArbMap.Default.GeneratorFor<DateTime>()
      select DateOnly.FromDateTime(dateTime);

  public static Arbitrary<DateOnly> Arbitrary() => Gen().ToArbitrary();
}
