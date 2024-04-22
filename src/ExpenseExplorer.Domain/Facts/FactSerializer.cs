namespace ExpenseExplorer.Domain.Facts;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ExpenseExplorer.Domain.Receipts.Facts;

public static class FactSerializer
{
  public static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      Receipts.Facts.ReceiptCreated receiptCreated => Serialize(receiptCreated),
      Receipts.Facts.PurchaseAdded purchaseAdded => Serialize(purchaseAdded),
      Receipts.Facts.StoreCorrected storeCorrected => Serialize(storeCorrected),
      Receipts.Facts.PurchaseDateChanged purchaseDateChanged => Serialize(purchaseDateChanged),
      _ => throw new UnreachableException(),
    };
  }

  public static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      FactTypes.ReceiptCreatedFactType => Deserialize<Receipts.Facts.ReceiptCreated>(data),
      FactTypes.PurchaseAddedFactType => Deserialize<Receipts.Facts.PurchaseAdded>(data),
      FactTypes.StoreCorrectedFactType => Deserialize<Receipts.Facts.StoreCorrected>(data),
      FactTypes.PurchaseDateChangedFactType => Deserialize<Receipts.Facts.PurchaseDateChanged>(data),
      _ => throw new UnreachableException(),
    };
  }

  private static byte[] Serialize<T>(T fact)
  {
    string json = JsonSerializer.Serialize(fact);
    return Encoding.UTF8.GetBytes(json);
  }

  private static T Deserialize<T>(byte[] data)
  {
    return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data))!;
  }
}
