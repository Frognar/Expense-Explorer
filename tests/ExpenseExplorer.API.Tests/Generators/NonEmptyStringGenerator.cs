namespace ExpenseExplorer.API.Tests.Generators;

public class NonEmptyStringGenerator {
  public static Arbitrary<string> NonEmptyStringGen() {
    return ArbMap.Default.ArbFor<NonEmptyString>()
      .Generator
      .Select(s => s.Get)
      .ToArbitrary();
  }
}
