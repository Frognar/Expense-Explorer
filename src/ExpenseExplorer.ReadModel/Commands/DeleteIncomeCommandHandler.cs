namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteIncomeCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<DeleteIncomeCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(DeleteIncomeCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    await _context.Incomes.Where(i => i.Id == command.IncomeId).ExecuteDeleteAsync(cancellationToken);
    return Unit.Instance;
  }
}
