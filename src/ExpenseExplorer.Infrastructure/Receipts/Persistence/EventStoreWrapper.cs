namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using EventStore.Client;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Events;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;
using static ExpenseExplorer.Domain.Events.EventSerializer;

#pragma warning disable CA1001
public class EventStoreWrapper(string connectionString) : IEventStore
#pragma warning restore CA1001
{
  private readonly EventStoreClient client = new(EventStoreClientSettings.Create(connectionString));

  public Task SaveEvents(Id id, IEnumerable<Fact> events)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(events);
    return AppendToStreamAsync(id.Value, events);
  }

  public Task<IEnumerable<Fact>> GetEvents(Id id)
  {
    ArgumentNullException.ThrowIfNull(id);
    return ReadFromStreamAsync(id.Value);
  }

  private static EventData ToEventData(Fact fact)
  {
    return new EventData(Uuid.NewUuid(), EventTypes.GetType(fact), Serialize(fact));
  }

  private async Task AppendToStreamAsync(string stream, IEnumerable<Fact> events)
  {
    var data = events.Select(ToEventData);
    _ = await client.AppendToStreamAsync(stream, StreamState.Any, data);
  }

  private async Task<IEnumerable<Fact>> ReadFromStreamAsync(string stream)
  {
    const Direction direction = Direction.Forwards;
    var streamResult = client.ReadStreamAsync(direction, stream, StreamPosition.Start);
    if (await streamResult.ReadState == ReadState.StreamNotFound)
    {
      return Enumerable.Empty<Fact>();
    }

    var events = await streamResult.Select(e => e.Event).ToListAsync();
    return events.Select(e => Deserialize(e.EventType, e.Data.ToArray()));
  }
}
