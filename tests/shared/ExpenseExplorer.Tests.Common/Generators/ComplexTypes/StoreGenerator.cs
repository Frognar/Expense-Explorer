namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class StoreGenerator
{
  public static Gen<Maybe<Store>> GenMaybe()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Store.TryCreate(str);

  public static Gen<Store> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<Store> Arbitrary() => Gen().ToArbitrary();
}
