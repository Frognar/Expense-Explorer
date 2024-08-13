using DotResult;
using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Infrastructure.Exceptions;
using FunctionalCore.Failures;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Infrastructure.Incomes.Persistence;

public sealed class EventStoreIncomeRepository(string connectionString) : IIncomeRepository, IDisposable
{
  private readonly EventStoreWrapper _eventStore = new(connectionString);

  public void Dispose()
  {
    _eventStore.Dispose();
  }

  public async Task<Result<Version>> SaveAsync(Income income, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(income);
      Version version = await _eventStore.SaveEventsAsync(
        income.Id,
        income.Version,
        income.UnsavedChanges,
        cancellationToken);

      return Success.From(version);
    }
    catch (FactSaveException ex)
    {
      return Fail.OfType<Version>(FailureFactory.Fatal(ex));
    }
  }

  public async Task<Result<Income>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      (List<Fact> facts, Version version) = await _eventStore.GetEventsAsync(id, cancellationToken);
      return facts.Count == 0
        ? Fail.OfType<Income>(FailureFactory.NotFound("Income not found", id.Value))
        : Income.Recreate(facts, version);
    }
    catch (FactSaveException ex)
    {
      return Fail.OfType<Income>(FailureFactory.Fatal(ex));
    }
  }
}
