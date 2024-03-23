namespace ExpenseExplorer.Domain.Events;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ExpenseExplorer.Domain.Receipts.Events;
using static ExpenseExplorer.Domain.Events.EventTypes;

public static class EventSerializer
{
  public static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      Receipts.Events.ReceiptCreated receiptCreated => Serialize(receiptCreated),
      Receipts.Events.PurchaseAdded purchaseAdded => Serialize(purchaseAdded),
      _ => throw new UnreachableException(),
    };
  }

  public static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      ReceiptCreatedEventType => Deserialize<ReceiptCreated>(data),
      PurchaseAddedEventType => Deserialize<PurchaseAdded>(data),
      _ => throw new UnreachableException(),
    };
  }

  private static byte[] Serialize<T>(T @event)
    where T : Fact
  {
    return JsonSerializer.SerializeToUtf8Bytes(@event);
  }

  private static Fact Deserialize<T>(byte[] data)
    where T : Fact
  {
    return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data))!;
  }
}
