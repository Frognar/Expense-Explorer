namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class CategoryGenerator
{
  public static Gen<Maybe<Category>> GenMaybe()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Category.TryCreate(str);

  public static Gen<Category> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Category> Arbitrary() => Gen().ToArbitrary();
}
