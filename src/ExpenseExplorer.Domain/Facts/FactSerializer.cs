namespace ExpenseExplorer.Domain.Facts;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public static class FactSerializer
{
  public static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      Receipts.Facts.ReceiptCreated receiptCreated => Serialize(Map(receiptCreated)),
      Receipts.Facts.PurchaseAdded purchaseAdded => Serialize(Map(purchaseAdded)),
      Receipts.Facts.StoreCorrected storeCorrected => Serialize(Map(storeCorrected)),
      Receipts.Facts.PurchaseDateChanged purchaseDateChanged => Serialize(Map(purchaseDateChanged)),
      _ => throw new UnreachableException(),
    };
  }

  public static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      FactTypes.ReceiptCreatedFactType => Map(Deserialize<SimpleReceiptCreated>(data)),
      FactTypes.PurchaseAddedFactType => Map(Deserialize<SimplePurchaseAdded>(data)),
      FactTypes.StoreCorrectedFactType => Map(Deserialize<SimpleStoreCorrected>(data)),
      FactTypes.PurchaseDateChangedFactType => Map(Deserialize<SimplePurchaseDateChanged>(data)),
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

  private static PurchaseAdded Map(SimplePurchaseAdded simplePurchaseAdded)
    => new(
      Id.Create(simplePurchaseAdded.ReceiptId),
      Purchase.Create(
        Id.Create(simplePurchaseAdded.PurchaseId),
        Item.Create(simplePurchaseAdded.Item),
        Category.Create(simplePurchaseAdded.Category),
        Quantity.Create(simplePurchaseAdded.Quantity),
        Money.Create(simplePurchaseAdded.UnitPrice),
        Money.Create(simplePurchaseAdded.TotalDiscount),
        Description.Create(simplePurchaseAdded.Description)));

  private static SimplePurchaseAdded Map(PurchaseAdded purchaseAdded)
    => new(
      purchaseAdded.ReceiptId.Value,
      purchaseAdded.Purchase.Id.Value,
      purchaseAdded.Purchase.Item.Name,
      purchaseAdded.Purchase.Category.Name,
      purchaseAdded.Purchase.Quantity.Value,
      purchaseAdded.Purchase.UnitPrice.Value,
      purchaseAdded.Purchase.TotalDiscount.Value,
      purchaseAdded.Purchase.Description.Value);

  private static StoreCorrected Map(SimpleStoreCorrected simpleStoreCorrected)
    => new(Id.Create(simpleStoreCorrected.ReceiptId), Store.Create(simpleStoreCorrected.Store));

  private static SimpleStoreCorrected Map(StoreCorrected storeCorrected)
    => new(storeCorrected.ReceiptId.Value, storeCorrected.Store.Name);

  private static PurchaseDateChanged Map(SimplePurchaseDateChanged simplePurchaseDateChanged)
    => new(
      Id.Create(simplePurchaseDateChanged.ReceiptId),
      PurchaseDate.Create(simplePurchaseDateChanged.PurchaseDate, simplePurchaseDateChanged.RequestedDate),
      simplePurchaseDateChanged.RequestedDate);

  private static SimplePurchaseDateChanged Map(PurchaseDateChanged purchaseDateChanged)
    => new(
      purchaseDateChanged.ReceiptId.Value,
      purchaseDateChanged.PurchaseDate.Date,
      purchaseDateChanged.RequestedDate);

  private static byte[] Serialize<T>(T fact)
  {
    string json = JsonSerializer.Serialize(fact);
    return Encoding.UTF8.GetBytes(json);
  }

  private static T Deserialize<T>(byte[] data)
  {
    return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data))!;
  }

  private sealed record SimpleReceiptCreated(string Id, string Store, DateOnly PurchaseDate, DateOnly CreatedDate);

  private sealed record SimplePurchaseAdded(
    string ReceiptId,
    string PurchaseId,
    string Item,
    string Category,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalDiscount,
    string Description);

  private sealed record SimpleStoreCorrected(string ReceiptId, string Store);

  private sealed record SimplePurchaseDateChanged(string ReceiptId, DateOnly PurchaseDate, DateOnly RequestedDate);
}
