namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using EventStore.Client;
using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

#pragma warning disable CA1001
public class EventStoreReceiptRepository(string connectionString) : IReceiptRepository
#pragma warning restore CA1001
{
  private readonly EventStoreClient client = new(EventStoreClientSettings.Create(connectionString));

  public async Task<Either<Failure, Unit>> Save(Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    await AppendToStreamAsync(receipt.Id.Value, receipt.UnsavedChanges);
    return Right.From<Failure, Unit>(Unit.Instance);
  }

  public async Task<Either<Failure, Receipt>> GetAsync(Id id)
  {
    ArgumentNullException.ThrowIfNull(id);
    var events = await ReadFromStreamAsync(id.Value);
    events = events.ToList();
    if (!events.Any())
    {
      return Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id));
    }

    var receipt = Receipt.Recreate(events);
    return Right.From<Failure, Receipt>(receipt);
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
