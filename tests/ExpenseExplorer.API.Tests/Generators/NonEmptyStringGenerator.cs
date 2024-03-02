namespace ExpenseExplorer.API.Tests.Generators;

public class NonEmptyStringGenerator {
  public static Arbitrary<string> NonEmptyStringGen() {
    return Arb.Default.NonEmptyString()
      .Generator
      .Select(s => s.Get)
      .ToArbitrary();
  }
}
