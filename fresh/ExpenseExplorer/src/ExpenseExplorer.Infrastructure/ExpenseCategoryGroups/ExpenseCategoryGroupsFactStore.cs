using DotResult;
using ExpenseExplorer.Application;
using ExpenseExplorer.Domain.ExpenseCategoryGroups;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Infrastructure.ExpenseCategoryGroups;

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
    Func<IEnumerable<Fact>, Result<ExpenseCategoryGroupType>> recreate,
    CancellationToken cancellationToken)
  {
    Result<(List<Fact> Facts, VersionType Version)> facts =
      await _eventStore.GetEventsAsync(streamId, cancellationToken);

    return facts.Bind(
      x => recreate(x.Facts)
        .Map(r => r with { Version = x.Version }));
  }

  public async Task<Result<ExpenseCategoryGroupType>> SaveAsync(
    string streamId,
    ExpenseCategoryGroupType value,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(value);
    Result<VersionType> version = await _eventStore.SaveEventsAsync(
      streamId,
      value.Version,
      value.UnsavedChanges.Facts,
      cancellationToken);

    return version.Map(v => value with { Version = v, UnsavedChanges = UnsavedChanges.Empty() });
  }
}
