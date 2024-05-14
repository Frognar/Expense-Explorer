namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class NonFutureDateGenerator
{
  public static Gen<Maybe<NonFutureDate>> GenMaybe()
    =>
      from date in DateOnlyGenerator.Gen()
      select NonFutureDate.TryCreate(date, date);

  public static Gen<NonFutureDate> Gen()
    =>
      from maybe in GenMaybe()
      select maybe.ForceValue();

  public static Arbitrary<NonFutureDate> Arbitrary() => Gen().ToArbitrary();
}
