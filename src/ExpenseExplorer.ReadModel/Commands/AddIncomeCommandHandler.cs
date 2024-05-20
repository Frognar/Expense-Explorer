namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

public sealed class AddIncomeCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<AddIncomeCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(AddIncomeCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbIncome purchase = new(
      command.Id,
      command.Source,
      command.Amount,
      command.Category,
      command.ReceivedDate,
      command.Description);

    _context.Incomes.Add(purchase);
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
