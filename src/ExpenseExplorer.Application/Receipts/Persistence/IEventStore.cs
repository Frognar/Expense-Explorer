namespace ExpenseExplorer.Application.Receipts.Persistence;

using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public interface IEventStore
{
  public Task<IEnumerable<Fact>> GetEvents(Id id);

  public Task SaveEvents(Id id, IEnumerable<Fact> events);
}
