namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class CategoryGenerator
{
  public static Gen<Category> Gen()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Category.Create(str);

  public static Arbitrary<Category> Arbitrary() => Gen().ToArbitrary();
}
