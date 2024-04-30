namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class CategoryGenerator
{
  public static Gen<Category> Gen()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Category.Create(str);

  public static Arbitrary<Category> Arbitrary() => Gen().ToArbitrary();
}
