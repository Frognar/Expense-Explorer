namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public static class InMemoryEventStore
{
  private static readonly List<(Id Id, Fact Fact)> Events = new();

  public static Task<IEnumerable<Fact>> GetEvents(Id id)
  {
    return Task.FromResult(
      Events
        .Where(x => x.Id == id)
        .Select(x => x.Fact));
  }

  public static Task SaveEvents(Id id, IEnumerable<Fact> events)
  {
    Events.AddRange(events.Select(fact => (id, fact)));
    return Task.CompletedTask;
  }
}
