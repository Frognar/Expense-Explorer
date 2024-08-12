using DotResult;
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

  public async Task<Result<VersionType>> SaveEventsAsync(
    string streamId,
    VersionType expectedVersion,
    IEnumerable<Fact> facts,
    CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(facts);
      WriteRequest request = new(streamId, new StreamRevision(expectedVersion.Value), facts.Select(ToEventData));
      WriteResult result = await AppendToStreamAsync(request, cancellationToken);
      return Version.Create(result.Version);
    }
    catch (FactSaveException ex)
    {
      return Failure.Fatal(message: ex.Message);
    }
  }

  public async Task<Result<(List<Fact> Facts, VersionType Version)>> GetEventsAsync(
    string streamId,
    CancellationToken cancellationToken)
  {
    try
    {
      ReadResult result = await ReadFromStreamAsync(new ReadQuery(streamId), cancellationToken);
      List<Fact> facts = result.Events.Select(e => Deserialize(e.EventType, e.Data.ToArray())).ToList();
      return (facts, Version.Create(result.Version));
    }
    catch (FactReadException ex)
    {
      return Failure.Fatal(message: ex.Message);
    }
  }

  private static EventData ToEventData(Fact fact)
  {
    return new EventData(Uuid.NewUuid(), GetFactType(fact), Serialize(fact));
  }

  private async Task<WriteResult> AppendToStreamAsync(WriteRequest request, CancellationToken cancellationToken)
  {
    try
    {
      IWriteResult writeResult = await _client.AppendToStreamAsync(
        request.Stream,
        request.Revision,
        request.Data,
        cancellationToken: cancellationToken);

      return new WriteResult(writeResult.NextExpectedStreamRevision);
    }
    catch (Exception ex)
    {
      throw FactSaveException.Wrap(ex);
    }
  }

  private async Task<ReadResult> ReadFromStreamAsync(ReadQuery query, CancellationToken cancellationToken)
  {
    try
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
    catch (Exception ex)
    {
      throw FactReadException.Wrap(ex);
    }
  }

  private readonly record struct WriteRequest(string Stream, StreamRevision Revision, IEnumerable<EventData> Data);

  private readonly record struct WriteResult(ulong Version);

  private readonly record struct ReadQuery(string Stream);

  private readonly record struct ReadResult(List<EventRecord> Events, ulong Version);
}
