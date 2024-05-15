namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class InvalidAddIncomeCommandGenerator
{
  public static Gen<AddIncomeCommand> Gen()
  {
    Gen<AddIncomeCommand> invalidSource =
      from source in EmptyOrWhiteSpaceStringGenerator.Gen()
      from amount in PositiveDecimalGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from receivedDate in DateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new AddIncomeCommand(source, amount, category, receivedDate, description, receivedDate);

    Gen<AddIncomeCommand> invalidAmount =
      from source in NonEmptyStringGenerator.Gen()
      from amount in NegativeDecimalGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from receivedDate in DateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new AddIncomeCommand(source, amount, category, receivedDate, description, receivedDate);

    Gen<AddIncomeCommand> invalidCategory =
      from source in NonEmptyStringGenerator.Gen()
      from amount in PositiveDecimalGenerator.Gen()
      from category in EmptyOrWhiteSpaceStringGenerator.Gen()
      from receivedDate in DateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new AddIncomeCommand(source, amount, category, receivedDate, description, receivedDate);

    Gen<AddIncomeCommand> invalidReceivedDate =
      from source in NonEmptyStringGenerator.Gen()
      from amount in PositiveDecimalGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from receivedDate in DateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new AddIncomeCommand(source, amount, category, receivedDate.AddDays(1), description, receivedDate);

    return FsCheck.Fluent.Gen.OneOf(
      invalidSource,
      invalidAmount,
      invalidCategory,
      invalidReceivedDate);
  }

  public static Arbitrary<AddIncomeCommand> Arbitrary() => Gen().ToArbitrary();
}
