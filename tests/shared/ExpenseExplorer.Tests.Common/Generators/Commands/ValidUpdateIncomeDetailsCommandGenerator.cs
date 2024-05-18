namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidUpdateIncomeDetailsCommandGenerator
{
  public static Gen<UpdateIncomeDetailsCommand> Gen()
    =>
      from source in NullableNonEmptyStringGenerator.Gen()
      from amount in NullableNonNegativeDecimalGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from receivedDate in NullableDateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdateIncomeDetailsCommand("incomeId", source, amount, category, receivedDate, description, receivedDate ?? DateOnly.MaxValue);

  public static Arbitrary<UpdateIncomeDetailsCommand> Arbitrary() => Gen().ToArbitrary();
}
