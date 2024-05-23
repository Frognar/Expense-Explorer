namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

public sealed class CorrectIncomeDescriptionCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<CorrectIncomeDescriptionCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(
    CorrectIncomeDescriptionCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbIncome? dbIncome = _context.Incomes.FirstOrDefault(r => r.Id == command.IncomeId);
    if (dbIncome is null)
    {
      throw new InvalidOperationException($"Income with id {command.IncomeId} not found.");
    }

    dbIncome.Description = command.Description;
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
