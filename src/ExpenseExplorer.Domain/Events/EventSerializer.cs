namespace ExpenseExplorer.Domain.Events;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;
using static ExpenseExplorer.Domain.Events.EventTypes;

public static class EventSerializer
{
  public static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      Receipts.Events.ReceiptCreated receiptCreated => Serialize(Map(receiptCreated)),
      Receipts.Events.PurchaseAdded purchaseAdded => Serialize(purchaseAdded),
      _ => throw new UnreachableException(),
    };
  }

  public static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      ReceiptCreatedEventType => Map(Deserialize<SimpleReceiptCreated>(data)),
      PurchaseAddedEventType => Deserialize<PurchaseAdded>(data),
      _ => throw new UnreachableException(),
    };
  }

  private static ReceiptCreated Map(SimpleReceiptCreated simpleReceiptCreated)
    => new(
      Id.Create(simpleReceiptCreated.Id),
      Store.Create(simpleReceiptCreated.Store),
      PurchaseDate.Create(simpleReceiptCreated.PurchaseDate, simpleReceiptCreated.CreatedDate),
      simpleReceiptCreated.CreatedDate);

  private static SimpleReceiptCreated Map(ReceiptCreated receiptCreated)
    => new(
      receiptCreated.Id.Value,
      receiptCreated.Store.Name,
      receiptCreated.PurchaseDate.Date,
      receiptCreated.CreatedDate);

  private static byte[] Serialize<T>(T @event)
  {
    string json = JsonSerializer.Serialize(@event);
    return Encoding.UTF8.GetBytes(json);
  }

  private static T Deserialize<T>(byte[] data)
  {
    return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data))!;
  }

  private sealed record SimpleReceiptCreated(string Id, string Store, DateOnly PurchaseDate, DateOnly CreatedDate);
}
