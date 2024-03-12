namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class CategoryGenerator
{
  public static Arbitrary<Category> CategoryGen()
  {
    return NonEmptyStringGenerator.NonEmptyStringGen()
      .Generator
      .Select(Category.Create)
      .ToArbitrary();
  }
}
