namespace ExpenseExplorer.Tests.Shared.Generators;

public class NonFutureDateOnlyGenerator {
  public static Arbitrary<DateOnly> NonFutureDateOnlyGen() {
    return ArbMap.Default.ArbFor<DateTime>()
      .Filter(dt => dt.Date <= today)
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}
