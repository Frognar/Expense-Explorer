namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using EventStore.Client;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

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
    return new EventData(Uuid.NewUuid(), fact.GetType().Name, Serialize(fact));
  }

  private static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      ReceiptCreated receiptCreated => JsonSerializer.SerializeToUtf8Bytes(receiptCreated),
      PurchaseAdded purchaseAdded => JsonSerializer.SerializeToUtf8Bytes(purchaseAdded),
      _ => throw new UnreachableException(),
    };
  }

  private static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      nameof(ReceiptCreated) => JsonSerializer.Deserialize<ReceiptCreated>(Encoding.UTF8.GetString(data))!,
      nameof(PurchaseAdded) => JsonSerializer.Deserialize<PurchaseAdded>(Encoding.UTF8.GetString(data))!,
      _ => throw new UnreachableException(),
    };
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
    var result = events.Select(e => Deserialize(e.EventType, e.Data.ToArray()));
    return result;
  }
}
