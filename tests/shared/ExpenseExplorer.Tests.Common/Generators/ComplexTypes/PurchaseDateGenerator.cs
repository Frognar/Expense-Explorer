namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class PurchaseDateGenerator
{
  public static Gen<Maybe<PurchaseDate>> GenMaybe()
    =>
      from date in DateOnlyGenerator.Gen()
      select PurchaseDate.TryCreate(date, date);

  public static Gen<PurchaseDate> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<PurchaseDate> Arbitrary() => Gen().ToArbitrary();
}
