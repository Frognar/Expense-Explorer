namespace ExpenseExplorer.Application.Incomes.Persistence;

using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public interface IIncomeRepository
{
  Task<Result<Version>> SaveAsync(Income income, CancellationToken cancellationToken);

  Task<Result<Income>> GetAsync(Id id, CancellationToken cancellationToken);
}
