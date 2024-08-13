using DotResult;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Application.Incomes.Persistence;

public interface IIncomeRepository
{
  Task<Result<Version>> SaveAsync(Income income, CancellationToken cancellationToken);

  Task<Result<Income>> GetAsync(Id id, CancellationToken cancellationToken);
}
