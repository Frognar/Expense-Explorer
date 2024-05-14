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
      _ => throw new UnreachableException(),
    };
  }
}
