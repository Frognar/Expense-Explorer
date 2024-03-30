namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using EventStore.Client;
using ExpenseExplorer.Application.Exceptions;
using ExpenseExplorer.Domain.Events;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;
using static ExpenseExplorer.Domain.Events.EventSerializer;

public sealed class EventStoreWrapper(string connectionString) : IDisposable
{
  private readonly EventStoreClient _client = new(EventStoreClientSettings.Create(connectionString));

  public void Dispose()
  {
    _client.Dispose();
  }

  public Task SaveEventsAsync(Id id, IEnumerable<Fact> events, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(id);
      ArgumentNullException.ThrowIfNull(events);
      return AppendToStreamAsync(id.Value, events, cancellationToken);
    }
    catch (Exception ex)
    {
      throw EventSaveException.Wrap(ex);
    }
  }

  public Task<IEnumerable<Fact>> GetEventsAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(id);
      return ReadFromStreamAsync(id.Value, cancellationToken);
    }
    catch (Exception ex)
    {
      throw EventReadException.Wrap(ex);
    }
  }

  private static EventData ToEventData(Fact fact)
  {
    return new EventData(Uuid.NewUuid(), EventTypes.GetType(fact), Serialize(fact));
  }

  private async Task AppendToStreamAsync(string stream, IEnumerable<Fact> events, CancellationToken cancellationToken)
  {
    var data = events.Select(ToEventData);
    _ = await _client.AppendToStreamAsync(stream, StreamState.Any, data, cancellationToken: cancellationToken);
  }

  private async Task<IEnumerable<Fact>> ReadFromStreamAsync(string stream, CancellationToken cancellationToken)
  {
    const Direction direction = Direction.Forwards;
    var streamResult = _client.ReadStreamAsync(
      direction,
      stream,
      StreamPosition.Start,
      cancellationToken: cancellationToken);

    if (await streamResult.ReadState == ReadState.StreamNotFound)
    {
      return Enumerable.Empty<Fact>();
    }

    var events = await streamResult.Select(e => e.Event).ToListAsync(cancellationToken);
    return events.Select(e => Deserialize(e.EventType, e.Data.ToArray()));
  }
}
