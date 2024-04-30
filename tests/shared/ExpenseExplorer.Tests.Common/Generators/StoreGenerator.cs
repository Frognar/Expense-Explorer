namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class StoreGenerator
{
  public static Gen<Store> Gen()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Store.Create(str);

  public static Arbitrary<Store> Arbitrary() => Gen().ToArbitrary();
}
