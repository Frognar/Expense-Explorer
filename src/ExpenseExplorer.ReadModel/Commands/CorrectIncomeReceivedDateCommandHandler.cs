using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed class CorrectIncomeReceivedDateCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<CorrectIncomeReceivedDateCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(
    CorrectIncomeReceivedDateCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbIncome? dbIncome = _context.Incomes.FirstOrDefault(r => r.Id == command.IncomeId);
    if (dbIncome is null)
    {
      throw new InvalidOperationException($"Income with id {command.IncomeId} not found.");
    }

    dbIncome.ReceivedDate = command.ReceivedDate;
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
