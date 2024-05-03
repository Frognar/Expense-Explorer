namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class ItemGenerator
{
  public static Gen<Maybe<Item>> GenMaybe()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Item.TryCreate(str);

  public static Gen<Item> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Item> Arbitrary() => Gen().ToArbitrary();
}
