namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class DescriptionGenerator
{
  public static Gen<Maybe<Description>> GenMaybe()
    =>
      from str in ArbMap.Default.GeneratorFor<string>()
      select Description.TryCreate(str);

  public static Gen<Description> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Description> Arbitrary() => Gen().ToArbitrary();
}
