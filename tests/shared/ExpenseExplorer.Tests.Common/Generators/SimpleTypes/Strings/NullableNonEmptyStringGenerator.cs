namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class NullableNonEmptyStringGenerator
{
  public static Gen<string?> Gen()
    =>
      from str in ArbMap.Default.GeneratorFor<NonWhiteSpaceString?>()
      select str?.Item;

  public static Arbitrary<string?> Arbitrary() => Gen().ToArbitrary();
}
