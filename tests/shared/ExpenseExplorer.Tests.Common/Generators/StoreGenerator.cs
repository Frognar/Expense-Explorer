namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class StoreGenerator
{
  public static Arbitrary<Store> StoreGen()
  {
    return NonEmptyStringGenerator.NonEmptyStringGen()
      .Generator
      .Select(Store.Create)
      .ToArbitrary();
  }
}
