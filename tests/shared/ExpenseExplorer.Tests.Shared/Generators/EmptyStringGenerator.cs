namespace ExpenseExplorer.Tests.Shared.Generators;

public class EmptyStringGenerator {
  public static Arbitrary<string> EmptyStringGen() {
    return ArbMap.Default.ArbFor<string>()
      .Filter(s => s is not null)
      .Filter(s => s.Trim() == "")
      .Generator
      .ToArbitrary();
  }
}
