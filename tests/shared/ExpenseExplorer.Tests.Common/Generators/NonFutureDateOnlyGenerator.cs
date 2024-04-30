namespace ExpenseExplorer.Tests.Common.Generators;

public static class NonFutureDateOnlyGenerator
{
  public static Gen<DateOnly> Gen()
    =>
      from dateTime in ArbMap.Default.GeneratorFor<DateTime>()
      where dateTime.Date <= Today
      select DateOnly.FromDateTime(dateTime);

  public static Arbitrary<DateOnly> Arbitrary() => Gen().ToArbitrary();
}
