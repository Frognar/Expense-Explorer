using DotResult;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Validations;

namespace ExpenseExplorer.Application.Incomes.Commands;

internal static class AddIncomeValidator
{
  public static Result<Income> Validate(AddIncomeCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Func<Source, Money, Category, NonFutureDate, Description, DateOnly, Income> createReceipt = Income.New;
    return createReceipt
      .Apply(ValidateSource(command.Source))
      .Apply(ValidateAmount(command.Amount))
      .Apply(ValidateCategory(command.Category))
      .Apply(ValidateReceivedDate(command.ReceivedDate, command.Today))
      .Apply(ValidateDescription(command.Description))
      .Apply(Validation.Succeeded(command.Today))
      .ToResult();
  }

  private static Validated<Source> ValidateSource(string source)
  {
    return Source.TryCreate(source)
      .Match(
        () => Validation.Failed<Source>(CommonFailures.EmptySource),
        Validation.Succeeded);
  }

  private static Validated<Money> ValidateAmount(decimal amount)
  {
    return Money.TryCreate(amount)
      .Match(
        () => Validation.Failed<Money>(CommonFailures.NegativeAmount),
        Validation.Succeeded);
  }

  private static Validated<Category> ValidateCategory(string category)
  {
    return Category.TryCreate(category)
      .Match(
        () => Validation.Failed<Category>(CommonFailures.EmptyCategory),
        Validation.Succeeded);
  }

  private static Validated<NonFutureDate> ValidateReceivedDate(DateOnly purchaseDate, DateOnly today)
  {
    return NonFutureDate.TryCreate(purchaseDate, today)
      .Match(
        () => Validation.Failed<NonFutureDate>(CommonFailures.FutureReceivedDate),
        Validation.Succeeded);
  }

  private static Validated<Description> ValidateDescription(string? description)
  {
    if (description is null)
    {
      return Validation.Succeeded(Description.Empty);
    }

    return Description.TryCreate(description)
      .Match(
        () => Validation.Failed<Description>(CommonFailures.InvalidDescription),
        Validation.Succeeded);
  }
}
