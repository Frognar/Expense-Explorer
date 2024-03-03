namespace ExpenseExplorer.API.Tests.Generators;

public class DateOnlyGenerator {
  public static Arbitrary<DateOnly> DateOnlyGen() {
    return ArbMap.Default.ArbFor<DateTime>()
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}
