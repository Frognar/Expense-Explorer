namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class EmptyStringGenerator
{
  public static Gen<string> Gen()
    =>
      from str in ArbMap.Default.GeneratorFor<string>()
      where str is not null && str.Trim().Length == 0
      select str;

  public static Arbitrary<string> Arbitrary() => Gen().ToArbitrary();
}
