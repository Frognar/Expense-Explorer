using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts;

public readonly record struct ReceiptType(
  ReceiptIdType Id,
  StoreType Store,
  NonFutureDateType PurchaseDate,
  PurchaseIdsType PurchaseIds,
  bool Deleted);

public static class Receipt
{
  public static ReceiptType Create(
    StoreType store,
    NonFutureDateType purchaseDate,
    PurchaseIdsType purchaseIds)
  {
    return new ReceiptType(ReceiptId.Unique(), store, purchaseDate, purchaseIds, false);
  }

  public static ReceiptType ChangeStore(
    this ReceiptType receipt,
    StoreType newStore)
  {
    return receipt.Store == newStore
      ? receipt
      : receipt with { Store = newStore };
  }

  public static ReceiptType ChangePurchaseDate(
    this ReceiptType receipt,
    NonFutureDateType newPurchaseDate)
  {
    return receipt.PurchaseDate == newPurchaseDate
      ? receipt
      : receipt with { PurchaseDate = newPurchaseDate };
  }

  public static ReceiptType Delete(
    this ReceiptType receipt)
  {
    return receipt with { Deleted = true };
  }

  public static ReceiptType AddPurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
  {
    return receipt.PurchaseIds.Contains(purchaseId)
      ? receipt
      : receipt with { PurchaseIds = receipt.PurchaseIds.Append(purchaseId) };
  }

  public static ReceiptType RemovePurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
  {
    return receipt.PurchaseIds.Contains(purchaseId)
      ? receipt
      : receipt with { PurchaseIds = receipt.PurchaseIds.Without(purchaseId) };
  }
}
