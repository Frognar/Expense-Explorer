namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class ItemGenerator
{
  public static Arbitrary<Item> ItemGen()
  {
    return NonEmptyStringGenerator.NonEmptyStringGen()
      .Generator
      .Select(Item.Create)
      .ToArbitrary();
  }
}
