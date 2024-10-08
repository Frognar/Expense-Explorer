using ExpenseExplorer.Domain.Incomes;

namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

public static class IncomeGenerator
{
  public static Gen<Income> Gen()
    =>
      from source in SourceGenerator.Gen()
      from amount in MoneyGenerator.Gen()
      from category in CategoryGenerator.Gen()
      from receivedDate in NonFutureDateGenerator.Gen()
      from description in DescriptionGenerator.Gen()
      select Income.New(source, amount, category, receivedDate, description, receivedDate.Date).ClearChanges();

  public static Arbitrary<Income> Arbitrary() => Gen().ToArbitrary();
}
