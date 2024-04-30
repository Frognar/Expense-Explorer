namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public static class PurchaseDateGenerator
{
  public static Gen<PurchaseDate> Gen()
    =>
      from date in NonFutureDateOnlyGenerator.Gen()
      select PurchaseDate.Create(date, TodayDateOnly);

  public static Arbitrary<PurchaseDate> Arbitrary() => Gen().ToArbitrary();
}
