namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore;

public sealed class DeleteIncomeCommandHandler(IIncomeRepository incomeRepository)
  : ICommandHandler<DeleteIncomeCommand, Result<Unit>>
{
  private readonly IIncomeRepository _incomeRepository = incomeRepository;

  public async Task<Result<Unit>> HandleAsync(DeleteIncomeCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return await (
      from receiptId in Id.TryCreate(command.IncomeId).ToResult(() => CommonFailures.InvalidIncomeId)
      from income in _incomeRepository.GetAsync(receiptId, cancellationToken)
      let deleted = income.Delete()
      from version in _incomeRepository.SaveAsync(deleted, cancellationToken)
      select Unit.Instance);
  }
}
