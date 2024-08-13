using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ExpenseExplorer.Domain.Incomes.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;

namespace ExpenseExplorer.Domain.Facts;

public static class FactSerializer
{
  public static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      ReceiptCreated receiptCreated => Serialize(receiptCreated),
      StoreCorrected storeCorrected => Serialize(storeCorrected),
      PurchaseDateChanged purchaseDateChanged => Serialize(purchaseDateChanged),
      PurchaseAdded purchaseAdded => Serialize(purchaseAdded),
      PurchaseDetailsChanged purchaseDetailsChanged => Serialize(purchaseDetailsChanged),
      PurchaseRemoved purchaseRemoved => Serialize(purchaseRemoved),
      ReceiptDeleted receiptDeleted => Serialize(receiptDeleted),
      IncomeCreated incomeCreated => Serialize(incomeCreated),
      SourceCorrected sourceCorrected => Serialize(sourceCorrected),
      AmountCorrected amountCorrected => Serialize(amountCorrected),
      CategoryCorrected categoryCorrected => Serialize(categoryCorrected),
      ReceivedDateCorrected receivedDateCorrected => Serialize(receivedDateCorrected),
      DescriptionCorrected descriptionCorrected => Serialize(descriptionCorrected),
      IncomeDeleted incomeDeleted => Serialize(incomeDeleted),
      _ => throw new UnreachableException(),
    };
  }

  public static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      FactTypes.ReceiptCreatedFactType => Deserialize<ReceiptCreated>(data),
      FactTypes.StoreCorrectedFactType => Deserialize<StoreCorrected>(data),
      FactTypes.PurchaseDateChangedFactType => Deserialize<PurchaseDateChanged>(data),
      FactTypes.PurchaseAddedFactType => Deserialize<PurchaseAdded>(data),
      FactTypes.PurchaseDetailsChangedFactType => Deserialize<PurchaseDetailsChanged>(data),
      FactTypes.PurchaseRemovedFactType => Deserialize<PurchaseRemoved>(data),
      FactTypes.ReceiptDeletedFactType => Deserialize<ReceiptDeleted>(data),
      FactTypes.IncomeCreatedFactType => Deserialize<IncomeCreated>(data),
      FactTypes.IncomeSourceCorrectedFactType => Deserialize<SourceCorrected>(data),
      FactTypes.IncomeAmountCorrectedFactType => Deserialize<AmountCorrected>(data),
      FactTypes.IncomeCategoryCorrectedFactType => Deserialize<CategoryCorrected>(data),
      FactTypes.IncomeReceivedDateCorrectedFactType => Deserialize<ReceivedDateCorrected>(data),
      FactTypes.IncomeDescriptionCorrectedFactType => Deserialize<DescriptionCorrected>(data),
      FactTypes.IncomeDeletedFactType => Deserialize<IncomeDeleted>(data),
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
