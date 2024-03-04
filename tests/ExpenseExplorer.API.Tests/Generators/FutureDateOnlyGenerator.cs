namespace ExpenseExplorer.API.Tests.Generators;

public class FutureDateOnlyGenerator {
  public static Arbitrary<DateOnly> DateOnlyGen() {
    return ArbMap.Default.ArbFor<DateTime>()
      .Filter(dt => dt.Date > today)
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}