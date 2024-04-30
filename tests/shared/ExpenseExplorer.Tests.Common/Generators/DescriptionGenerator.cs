namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class DescriptionGenerator
{
  public static Gen<Description> Gen()
    =>
      from str in ArbMap.Default.GeneratorFor<string>()
      select Description.Create(str);

  public static Arbitrary<Description> Arbitrary() => Gen().ToArbitrary();
}
