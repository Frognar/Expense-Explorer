namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

public sealed class CorrectIncomeAmountCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<CorrectIncomeAmountCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(CorrectIncomeAmountCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbIncome? dbIncome = _context.Incomes.FirstOrDefault(r => r.Id == command.IncomeId);
    if (dbIncome is null)
    {
      throw new InvalidOperationException($"Income with id {command.IncomeId} not found.");
    }

    dbIncome.Amount = command.Amount;
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
