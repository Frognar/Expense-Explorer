namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public static class StoreGenerator
{
  public static Gen<Store> Gen()
    =>
      from str in NonEmptyStringGenerator.Gen()
      select Store.Create(str);

  public static Arbitrary<Store> Arbitrary() => Gen().ToArbitrary();
}
