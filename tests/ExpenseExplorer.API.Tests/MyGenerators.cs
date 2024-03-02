namespace ExpenseExplorer.API.Tests;

public class MyGenerators {
  public static Arbitrary<string> NonEmptyStringGen() {
    return Arb.Default.String().Filter(s => string.IsNullOrWhiteSpace(s) == false);
  }

  public static Arbitrary<DateOnly> DateOnlyGen() {
    return Arb.Default.DateTime().Generator.Select(DateOnly.FromDateTime).ToArbitrary();
  }
}
