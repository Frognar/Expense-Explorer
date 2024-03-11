namespace ExpenseExplorer.Tests.Common.Generators;

public static class EmptyStringGenerator
{
  public static Arbitrary<string> EmptyStringGen()
  {
    return ArbMap.Default.ArbFor<string>()
      .Filter(s => s is not null && s.Trim().Length == 0);
  }
}
