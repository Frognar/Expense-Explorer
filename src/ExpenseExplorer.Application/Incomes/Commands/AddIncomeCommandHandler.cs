namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public sealed class AddIncomeCommandHandler(IIncomeRepository incomeRepository)
  : ICommandHandler<AddIncomeCommand, Result<Income>>
{
  private readonly IIncomeRepository _incomeRepository = incomeRepository;

  public async Task<Result<Income>> HandleAsync(AddIncomeCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    var resultOfIncome = (
        from source in Source.TryCreate(command.Source)
        from amount in Money.TryCreate(command.Amount)
        from category in Category.TryCreate(command.Category)
        from receivedDate in NonFutureDate.TryCreate(command.ReceivedDate, command.Today)
        from description in Description.TryCreate(command.Description)
        select Income.New(source, amount, category, receivedDate, description, command.Today))
      .ToResult(() => throw new ArgumentException("Something went wrong"));

    return await (
      from income in resultOfIncome
      from version in _incomeRepository.SaveAsync(income, cancellationToken)
      select income.WithVersion(version).ClearChanges());
  }
}
