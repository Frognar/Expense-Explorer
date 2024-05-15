namespace ExpenseExplorer.Tests.Common.Generators.Commands;

public static class ValidAddIncomeCommandGenerator
{
  public static Gen<AddIncomeCommand> Gen()
    =>
      from source in NonEmptyStringGenerator.Gen()
      from amount in NonNegativeDecimalGenerator.Gen()
      from category in NonEmptyStringGenerator.Gen()
      from receivedDate in DateOnlyGenerator.Gen()
      from description in ArbMap.Default.GeneratorFor<string>()
      select new AddIncomeCommand(source, amount, category, receivedDate, description, receivedDate);

  public static Arbitrary<AddIncomeCommand> Arbitrary() => Gen().ToArbitrary();
}
