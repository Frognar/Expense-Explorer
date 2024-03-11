namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class PurchaseDateGenerator
{
  public static Arbitrary<PurchaseDate> PurchaseDateGen()
  {
    return NonFutureDateOnlyGenerator.NonFutureDateOnlyGen()
      .Generator
      .Select(date => PurchaseDate.Create(date, TodayDateOnly))
      .ToArbitrary();
  }
}
