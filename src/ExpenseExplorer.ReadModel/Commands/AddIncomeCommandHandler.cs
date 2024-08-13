using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed class AddIncomeCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<AddIncomeCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(AddIncomeCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbIncome income = new(
      command.IncomeId,
      command.Source,
      command.Amount,
      command.Category,
      command.ReceivedDate,
      command.Description);

    _context.Incomes.Add(income);
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
