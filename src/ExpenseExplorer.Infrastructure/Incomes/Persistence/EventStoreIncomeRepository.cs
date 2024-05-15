namespace ExpenseExplorer.Infrastructure.Incomes.Persistence;

using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public sealed class EventStoreIncomeRepository(string connectionString) : IIncomeRepository, IDisposable
{
  private readonly EventStoreWrapper _eventStore = new(connectionString);

  public void Dispose()
  {
    _eventStore.Dispose();
  }

  public Task<Result<Version>> SaveAsync(Income income, CancellationToken cancellationToken)
  {
    return Task.FromResult(Fail.OfType<Version>(Failure.Fatal(new NotImplementedException())));
  }

  public Task<Result<Income>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    return Task.FromResult(Fail.OfType<Income>(Failure.Fatal(new NotImplementedException())));
  }
}
