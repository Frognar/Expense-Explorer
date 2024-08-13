using EventStore.Client;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Infrastructure.Exceptions;
using static ExpenseExplorer.Domain.Facts.FactSerializer;
using static ExpenseExplorer.Domain.Facts.FactTypes;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Infrastructure;

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
    IEnumerable<Fact> facts,
    CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(id);
      ArgumentNullException.ThrowIfNull(facts);
      WriteRequest request = new(id.Value, new StreamRevision(expectedVersion.Value), facts.Select(ToEventData));
      WriteResult result = await AppendToStreamAsync(request, cancellationToken);
      return Version.Create(result.Version);
    }
    catch (Exception ex)
    {
      throw FactSaveException.Wrap(ex);
    }
  }

  public async Task<(List<Fact> Facts, Version Version)> GetEventsAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(id);
      ReadResult result = await ReadFromStreamAsync(new ReadQuery(id.Value), cancellationToken);
      List<Fact> facts = result.Events.Select(e => Deserialize(e.EventType, e.Data.ToArray())).ToList();
      return (facts, Version.Create(result.Version));
    }
    catch (Exception ex)
    {
      throw FactReadException.Wrap(ex);
    }
  }

  private static EventData ToEventData(Fact fact)
  {
    return new EventData(Uuid.NewUuid(), GetFactType(fact), Serialize(fact));
  }

  private async Task<WriteResult> AppendToStreamAsync(WriteRequest request, CancellationToken cancellationToken)
  {
    IWriteResult writeResult = await _client.AppendToStreamAsync(
      request.Stream,
      request.Revision,
      request.Data,
      cancellationToken: cancellationToken);

    return new WriteResult(writeResult.NextExpectedStreamRevision);
  }

  private async Task<ReadResult> ReadFromStreamAsync(ReadQuery query, CancellationToken cancellationToken)
  {
    const Direction direction = Direction.Forwards;
    StreamPosition position = StreamPosition.Start;
    var streamResult = _client.ReadStreamAsync(direction, query.Stream, position, cancellationToken: cancellationToken);
    if (await streamResult.ReadState == ReadState.StreamNotFound)
    {
      return new ReadResult([], StreamPosition.Start);
    }

    List<EventRecord> events = await streamResult.Select(e => e.Event).ToListAsync(cancellationToken);
    return new ReadResult(events, events.Max(e => e.EventNumber));
  }

  private readonly record struct WriteRequest(string Stream, StreamRevision Revision, IEnumerable<EventData> Data);

  private readonly record struct WriteResult(ulong Version);

  private readonly record struct ReadQuery(string Stream);

  private readonly record struct ReadResult(List<EventRecord> Events, ulong Version);
}
