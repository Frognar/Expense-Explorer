namespace ExpenseExplorer.Domain.Facts;

using System.Diagnostics;
using ExpenseExplorer.Domain.Incomes.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;

public static class FactTypes
{
  public const string ReceiptCreatedFactType = "RECEIPT_CREATED";
  public const string StoreCorrectedFactType = "STORE_CORRECTED";
  public const string PurchaseDateChangedFactType = "PURCHASE_DATE_CHANGED";
  public const string PurchaseAddedFactType = "PURCHASE_ADDED";
  public const string PurchaseDetailsChangedFactType = "PURCHASE_DETAILS_CHANGED";
  public const string PurchaseRemovedFactType = "PURCHASE_REMOVED";
  public const string ReceiptDeletedFactType = "RECEIPT_DELETED";
  public const string IncomeCreatedFactType = "INCOME_CREATED";
  public const string IncomeSourceCorrectedFactType = "INCOME_SOURCE_CORRECTED";
  public const string IncomeAmountCorrectedFactType = "INCOME_AMOUNT_CORRECTED";
  public const string IncomeCategoryCorrectedFactType = "INCOME_CATEGORY_CORRECTED";
  public const string IncomeReceivedDateCorrectedFactType = "INCOME_RECEIVED_DATE_CORRECTED";
  public const string IncomeDescriptionCorrectedFactType = "INCOME_DESCRIPTION_CORRECTED";

  public static string GetFactType(Fact fact)
  {
    return fact switch
    {
      ReceiptCreated => ReceiptCreatedFactType,
      StoreCorrected => StoreCorrectedFactType,
      PurchaseDateChanged => PurchaseDateChangedFactType,
      PurchaseAdded => PurchaseAddedFactType,
      PurchaseDetailsChanged => PurchaseDetailsChangedFactType,
      PurchaseRemoved => PurchaseRemovedFactType,
      ReceiptDeleted => ReceiptDeletedFactType,
      IncomeCreated => IncomeCreatedFactType,
      SourceCorrected => IncomeSourceCorrectedFactType,
      AmountCorrected => IncomeAmountCorrectedFactType,
      CategoryCorrected => IncomeCategoryCorrectedFactType,
      ReceivedDateCorrected => IncomeReceivedDateCorrectedFactType,
      DescriptionCorrected => IncomeDescriptionCorrectedFactType,
      _ => throw new UnreachableException(),
    };
  }
}
