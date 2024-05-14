namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class SourceGenerator
{
  public static Gen<Maybe<Source>> GenMaybe()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Source.TryCreate(str);

  public static Gen<Source> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Source> Arbitrary() => Gen().ToArbitrary();
}
