using DotResult;
using ExpenseExplorer.Application;
using ExpenseExplorer.Domain.ExpenseCategoryGroups;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Infrastructure;

public sealed class ExpenseCategoryGroupsFactStore(string connectionString)
  : IFactStore<ExpenseCategoryGroupType>, IDisposable
{
  private readonly EventStoreWrapper _eventStore = new(connectionString);

  public void Dispose()
  {
    _eventStore.Dispose();
  }

  public async Task<Result<ExpenseCategoryGroupType>> ReadAsync(
    string streamId,
    CancellationToken cancellationToken)
  {
    Result<(List<Fact> Facts, VersionType Version)> facts =
      await _eventStore.GetEventsAsync(streamId, cancellationToken);

    return facts.Bind(
      x => ExpenseCategoryGroup.Recreate(x.Facts)
        .Map(r => r with { Version = x.Version }));
  }

  public async Task<Result<ExpenseCategoryGroupType>> SaveAsync(
    ExpenseCategoryGroupType value,
    CancellationToken cancellationToken)
  {
    Result<VersionType> version = await _eventStore.SaveEventsAsync(
      value.Id.Value,
      value.Version,
      value.UnsavedChanges.Facts,
      cancellationToken);

    return version.Map(v => value with { Version = v, UnsavedChanges = UnsavedChanges.Empty() });
  }
}
