namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class DescriptionGenerator
{
  public static Arbitrary<Description> DescriptionGen()
  {
    return ArbMap.Default.ArbFor<string>()
      .Generator
      .Select(Description.Create)
      .ToArbitrary();
  }
}
