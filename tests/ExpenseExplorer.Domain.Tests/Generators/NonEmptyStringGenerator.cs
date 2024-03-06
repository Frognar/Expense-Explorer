namespace ExpenseExplorer.Domain.Tests.Generators;

public class NonEmptyStringGenerator {
  public static Arbitrary<string> NonEmptyStringGen() {
    return ArbMap.Default.ArbFor<NonWhiteSpaceString>()
      .Generator
      .Select(str => str.Item)
      .ToArbitrary();
  }
}
