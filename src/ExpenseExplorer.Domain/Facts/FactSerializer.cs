namespace ExpenseExplorer.Domain.Facts;

using System.Diagnostics;
using System.Text;
using System.Text.Json;

public static class FactSerializer
{
  public static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      Receipts.Facts.ReceiptCreated receiptCreated => Serialize(receiptCreated),
      Receipts.Facts.StoreCorrected storeCorrected => Serialize(storeCorrected),
      Receipts.Facts.PurchaseDateChanged purchaseDateChanged => Serialize(purchaseDateChanged),
      Receipts.Facts.PurchaseAdded purchaseAdded => Serialize(purchaseAdded),
      Receipts.Facts.PurchaseDetailsChanged purchaseDetailsChanged => Serialize(purchaseDetailsChanged),
      Receipts.Facts.PurchaseRemoved purchaseRemoved => Serialize(purchaseRemoved),
      Receipts.Facts.ReceiptDeleted receiptDeleted => Serialize(receiptDeleted),
      Incomes.Facts.IncomeCreated incomeCreated => Serialize(incomeCreated),
      Incomes.Facts.SourceCorrected sourceCorrected => Serialize(sourceCorrected),
      Incomes.Facts.AmountCorrected amountCorrected => Serialize(amountCorrected),
      Incomes.Facts.CategoryCorrected categoryCorrected => Serialize(categoryCorrected),
      Incomes.Facts.ReceivedDateCorrected receivedDateCorrected => Serialize(receivedDateCorrected),
      Incomes.Facts.DescriptionCorrected descriptionCorrected => Serialize(descriptionCorrected),
      _ => throw new UnreachableException(),
    };
  }

  public static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      FactTypes.ReceiptCreatedFactType => Deserialize<Receipts.Facts.ReceiptCreated>(data),
      FactTypes.StoreCorrectedFactType => Deserialize<Receipts.Facts.StoreCorrected>(data),
      FactTypes.PurchaseDateChangedFactType => Deserialize<Receipts.Facts.PurchaseDateChanged>(data),
      FactTypes.PurchaseAddedFactType => Deserialize<Receipts.Facts.PurchaseAdded>(data),
      FactTypes.PurchaseDetailsChangedFactType => Deserialize<Receipts.Facts.PurchaseDetailsChanged>(data),
      FactTypes.PurchaseRemovedFactType => Deserialize<Receipts.Facts.PurchaseRemoved>(data),
      FactTypes.ReceiptDeletedFactType => Deserialize<Receipts.Facts.ReceiptDeleted>(data),
      FactTypes.IncomeCreatedFactType => Deserialize<Incomes.Facts.IncomeCreated>(data),
      FactTypes.IncomeSourceCorrectedFactType => Deserialize<Incomes.Facts.SourceCorrected>(data),
      FactTypes.IncomeAmountCorrectedFactType => Deserialize<Incomes.Facts.AmountCorrected>(data),
      FactTypes.IncomeCategoryCorrectedFactType => Deserialize<Incomes.Facts.CategoryCorrected>(data),
      FactTypes.IncomeReceivedDateCorrectedFactType => Deserialize<Incomes.Facts.ReceivedDateCorrected>(data),
      FactTypes.IncomeDescriptionCorrectedFactType => Deserialize<Incomes.Facts.DescriptionCorrected>(data),
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
