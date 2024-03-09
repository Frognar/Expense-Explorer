namespace ExpenseExplorer.Tests.Common.Generators;

public static class FutureDateOnlyGenerator
{
  public static Arbitrary<DateOnly> FutureDateOnlyGen()
  {
    return ArbMap.Default.ArbFor<DateTime>()
      .Filter(dt => dt.Date > Today)
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}
