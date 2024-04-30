namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class PurchaseDateGenerator
{
  public static Gen<PurchaseDate> Gen()
    =>
      from date in NonFutureDateOnlyGenerator.Gen()
      select PurchaseDate.Create(date, TodayDateOnly);

  public static Arbitrary<PurchaseDate> Arbitrary() => Gen().ToArbitrary();
}
