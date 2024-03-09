namespace ExpenseExplorer.Tests.Common.Generators;

public static class NonFutureDateOnlyGenerator
{
  public static Arbitrary<DateOnly> NonFutureDateOnlyGen()
  {
    return ArbMap.Default.ArbFor<DateTime>()
      .Filter(dt => dt.Date <= Today)
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}
