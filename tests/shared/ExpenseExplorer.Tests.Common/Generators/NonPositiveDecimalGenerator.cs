namespace ExpenseExplorer.Tests.Common.Generators;

public static class NonPositiveDecimalGenerator
{
  public static Arbitrary<decimal> NonPositiveGen()
  {
    return ArbMap.Default.ArbFor<decimal>()
      .Filter(d => d <= 0);
  }
}
