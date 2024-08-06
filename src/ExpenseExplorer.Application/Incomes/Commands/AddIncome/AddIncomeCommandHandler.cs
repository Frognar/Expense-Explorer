namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Domain.Incomes;

public sealed class AddIncomeCommandHandler(IIncomeRepository incomeRepository)
  : ICommandHandler<AddIncomeCommand, Result<Income>>
{
  private readonly IIncomeRepository _incomeRepository = incomeRepository;

  public async Task<Result<Income>> HandleAsync(AddIncomeCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return await (
      from income in AddIncomeValidator.Validate(command)
      from version in _incomeRepository.SaveAsync(income, cancellationToken)
      select income.WithVersion(version).ClearChanges());
  }
}
