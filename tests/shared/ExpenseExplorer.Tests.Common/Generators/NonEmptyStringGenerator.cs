namespace ExpenseExplorer.Tests.Common.Generators;

public static class NonEmptyStringGenerator
{
  public static Gen<string> Gen()
    =>
      from str in ArbMap.Default.GeneratorFor<NonWhiteSpaceString>()
      select str.Item;

  public static Arbitrary<string> Arbitrary() => Gen().ToArbitrary();
}
