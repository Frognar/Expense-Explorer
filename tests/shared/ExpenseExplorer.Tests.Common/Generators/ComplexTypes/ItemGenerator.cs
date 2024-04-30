namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class ItemGenerator
{
  public static Gen<Item> Gen()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Item.Create(str);

  public static Arbitrary<Item> Arbitrary() => Gen().ToArbitrary();
}
