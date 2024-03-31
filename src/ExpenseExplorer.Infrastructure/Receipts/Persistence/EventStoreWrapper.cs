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

  public async Task<Version> SaveEventsAsync(
    Id id,
    Version expectedVersion,
    IEnumerable<Fact> events,
    CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(id);
      ArgumentNullException.ThrowIfNull(events);
      var data = events.Select(ToEventData);
      var streamRevision = await AppendToStreamAsync(
        id.Value,
        new StreamRevision(expectedVersion.Value),
        data,
        cancellationToken);

      return Version.Create(streamRevision.ToUInt64());
    }
    catch (Exception ex)
    {
      throw EventSaveException.Wrap(ex);
    }
  }

  public async Task<(List<Fact> Facts, Version Version)> GetEventsAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(id);
      (List<EventRecord> events, StreamPosition position) = await ReadFromStreamAsync(id.Value, cancellationToken);
      List<Fact> facts = events.Select(e => Deserialize(e.EventType, e.Data.ToArray())).ToList();
      Version version = Version.Create(position.ToUInt64());
      return (facts, version);
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

  private async Task<StreamRevision> AppendToStreamAsync(
    string stream,
    StreamRevision revision,
    IEnumerable<EventData> events,
    CancellationToken cancellationToken)
  {
    var writeResult = await _client.AppendToStreamAsync(stream, revision, events, cancellationToken: cancellationToken);
    return writeResult.NextExpectedStreamRevision;
  }

  private async Task<(List<EventRecord> Events, StreamPosition Position)> ReadFromStreamAsync(
    string stream,
    CancellationToken cancellationToken)
  {
    const Direction direction = Direction.Forwards;
    var streamResult = _client.ReadStreamAsync(
      direction,
      stream,
      StreamPosition.Start,
      cancellationToken: cancellationToken);

    if (await streamResult.ReadState == ReadState.StreamNotFound)
    {
      return ([], StreamPosition.Start);
    }

    List<EventRecord> events = await streamResult.Select(e => e.Event).ToListAsync(cancellationToken);
    StreamPosition position = events.Max(e => e.EventNumber);
    return (events, position);
  }
}
