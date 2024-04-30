namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class ItemGenerator
{
  public static Gen<Item> Gen()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Item.Create(str);

  public static Arbitrary<Item> Arbitrary() => Gen().ToArbitrary();
}
