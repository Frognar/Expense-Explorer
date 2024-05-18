namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidUpdateIncomeDetailsCommandGenerator
{
  public static Gen<UpdateIncomeDetailsCommand> Gen()
  {
    Gen<UpdateIncomeDetailsCommand> invalidSource =
      from source in EmptyOrWhiteSpaceStringGenerator.Gen()
      from amount in NullableNonNegativeDecimalGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from receivedDate in NullableDateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdateIncomeDetailsCommand("incomeId", source, amount, category, receivedDate, description, receivedDate ?? DateOnly.MaxValue);

    Gen<UpdateIncomeDetailsCommand> invalidAmount =
      from source in NullableNonEmptyStringGenerator.Gen()
      from amount in NegativeDecimalGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from receivedDate in NullableDateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdateIncomeDetailsCommand("incomeId", source, amount, category, receivedDate, description, receivedDate ?? DateOnly.MaxValue);

    Gen<UpdateIncomeDetailsCommand> invalidCategory =
      from source in NullableNonEmptyStringGenerator.Gen()
      from amount in NullableNonNegativeDecimalGenerator.Gen()
      from category in EmptyOrWhiteSpaceStringGenerator.Gen()
      from receivedDate in NullableDateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdateIncomeDetailsCommand("incomeId", source, amount, category, receivedDate, description, receivedDate ?? DateOnly.MaxValue);

    Gen<UpdateIncomeDetailsCommand> invalidReceivedDate =
      from source in NullableNonEmptyStringGenerator.Gen()
      from amount in NullableNonNegativeDecimalGenerator.Gen()
      from category in NullableNonEmptyStringGenerator.Gen()
      from receivedDate in DateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new UpdateIncomeDetailsCommand("incomeId", source, amount, category, receivedDate, description, receivedDate.AddDays(-1));

    return FsCheck.Fluent.Gen.OneOf(invalidSource, invalidAmount, invalidCategory, invalidReceivedDate);
  }

  public static Arbitrary<UpdateIncomeDetailsCommand> Arbitrary() => Gen().ToArbitrary();
}
