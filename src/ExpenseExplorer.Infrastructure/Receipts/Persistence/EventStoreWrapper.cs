namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using EventStore.Client;
using ExpenseExplorer.Application.Exceptions;
using ExpenseExplorer.Domain.Events;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;
using static ExpenseExplorer.Domain.Events.EventSerializer;

public class EventStoreWrapper(string connectionString) : IDisposable
{
  private readonly EventStoreClient client = new(EventStoreClientSettings.Create(connectionString));

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public Task SaveEvents(Id id, IEnumerable<Fact> events)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(id);
      ArgumentNullException.ThrowIfNull(events);
      return AppendToStreamAsync(id.Value, events);
    }
    catch (Exception ex)
    {
      throw EventSaveException.Wrap(ex);
    }
  }

  public Task<IEnumerable<Fact>> GetEvents(Id id)
  {
    ArgumentNullException.ThrowIfNull(id);
    return ReadFromStreamAsync(id.Value);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      client.Dispose();
    }
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
