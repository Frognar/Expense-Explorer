namespace ExpenseExplorer.API.Tests.Generators;

public class NonFutureDateOnlyGenerator {
  public static Arbitrary<DateOnly> DateOnlyGen() {
    return Arb.Default.DateTime()
      .Filter(dt => dt <= DateTime.Now)
      .Generator
      .Select(DateOnly.FromDateTime)
      .ToArbitrary();
  }
}
