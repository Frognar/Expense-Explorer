namespace ExpenseExplorer.Tests.Common.Generators;

public static class DateOnlyGenerator
{
  public static Arbitrary<DateOnly> DateOnlyGen()
  {
    return ArbMap.Default.ArbFor<DateTime>()
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}
