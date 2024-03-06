namespace ExpenseExplorer.Tests.Shared.Generators;

public class FutureDateOnlyGenerator {
  public static Arbitrary<DateOnly> FutureDateOnlyGen() {
    return ArbMap.Default.ArbFor<DateTime>()
      .Filter(dt => dt.Date > today)
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}
