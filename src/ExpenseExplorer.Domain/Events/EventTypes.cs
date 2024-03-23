namespace ExpenseExplorer.Domain.Events;

using System.Diagnostics;
using ExpenseExplorer.Domain.Receipts.Events;

public static class EventTypes
{
  public const string ReceiptCreatedEventType = "RECEIPT_CREATED";
  public const string PurchaseAddedEventType = "PURCHASE_ADDED";

  public static string GetType(Fact fact)
  {
    return fact switch
    {
      ReceiptCreated => ReceiptCreatedEventType,
      PurchaseAdded => PurchaseAddedEventType,
      _ => throw new UnreachableException(),
    };
  }
}
