namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public sealed class AddIncomeCommandHandler(IIncomeRepository incomeRepository)
  : ICommandHandler<AddIncomeCommand, Result<Income>>
{
#pragma warning disable S1144
#pragma warning disable CA1823
  private readonly IIncomeRepository _incomeRepository = incomeRepository;
#pragma warning restore CA1823
#pragma warning restore S1144

  public Task<Result<Income>> HandleAsync(AddIncomeCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return
      Task.FromResult(
        (
          from source in Source.TryCreate(command.Source)
          from amount in Money.TryCreate(command.Amount)
          from category in Category.TryCreate(command.Category)
          from receivedDate in NonFutureDate.TryCreate(command.ReceivedDate, command.Today)
          from description in Description.TryCreate(command.Description)
          select Income.New(source, amount, category, receivedDate, description, command.Today))
        .ToResult(() => throw new ArgumentException("Something went wrong")));
  }
}
