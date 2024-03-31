namespace ExpenseExplorer.Domain.Facts;

using System.Diagnostics;
using ExpenseExplorer.Domain.Receipts.Facts;

public static class FactTypes
{
  public const string ReceiptCreatedFactType = "RECEIPT_CREATED";
  public const string PurchaseAddedFactType = "PURCHASE_ADDED";
  public const string StoreCorrectedFactType = "STORE_CORRECTED";

  public static string GetFactType(Fact fact)
  {
    return fact switch
    {
      ReceiptCreated => ReceiptCreatedFactType,
      PurchaseAdded => PurchaseAddedFactType,
      StoreCorrected => StoreCorrectedFactType,
      _ => throw new UnreachableException(),
    };
  }
}