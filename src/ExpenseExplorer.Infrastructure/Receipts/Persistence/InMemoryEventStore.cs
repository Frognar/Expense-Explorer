namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application.Exceptions;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public static class InMemoryEventStore
{
  private static readonly List<(Id Id, Fact Fact)> _events = new();

  public static Task<IEnumerable<Fact>> GetEvents(Id id)
  {
    try
    {
      return Task.FromResult(
        _events
          .Where(x => x.Id == id)
          .Select(x => x.Fact));
    }
    catch (Exception ex)
    {
      throw EventReadException.Wrap(ex);
    }
  }

  public static Task SaveEvents(Id id, IEnumerable<Fact> events)
  {
    try
    {
      _events.AddRange(events.Select(fact => (id, fact)));
      return Task.CompletedTask;
    }
    catch (Exception ex)
    {
      throw EventSaveException.Wrap(ex);
    }
  }
}
