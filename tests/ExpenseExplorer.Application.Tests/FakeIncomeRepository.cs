namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using ExpenseExplorer.Application.Incomes.Persistence;
using FunctionalCore.Failures;

internal sealed class FakeIncomeRepository : Collection<Income>, IIncomeRepository
{
  public Task<Result<Version>> SaveAsync(Income income, CancellationToken cancellationToken)
  {
    Version version = Version.Create(income.Version.Value + (ulong)income.UnsavedChanges.Count());
    if (this.FirstOrDefault(r => r.Id == income.Id) is not null)
    {
      this[0] = income.WithVersion(version).ClearChanges();
    }
    else
    {
      Add(income.WithVersion(version).ClearChanges());
    }

    return Task.FromResult(Success.From(version));
  }

  public Task<Result<Income>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    Income? income = this.SingleOrDefault(r => r.Id == id);
    return income is null
      ? Task.FromResult(Fail.OfType<Income>(FailureFactory.NotFound("Income not found", id.Value)))
      : Task.FromResult(Success.From(income));
  }
}
