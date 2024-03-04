namespace ExpenseExplorer.API.Tests.Generators;

public class EmptyStringGenerator {
  public static Arbitrary<string> NonEmptyStringGen() {
    return ArbMap.Default.ArbFor<string>()
      .Filter(s => s is not null)
      .Filter(s => s.Trim() == "")
      .Generator
      .ToArbitrary();
  }
}
