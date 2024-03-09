namespace ExpenseExplorer.Tests.Common.Generators;

public static class NonEmptyStringGenerator
{
  public static Arbitrary<string> NonEmptyStringGen()
  {
    return ArbMap.Default.ArbFor<NonWhiteSpaceString>()
      .Generator
      .Select(str => str.Item)
      .ToArbitrary();
  }
}
