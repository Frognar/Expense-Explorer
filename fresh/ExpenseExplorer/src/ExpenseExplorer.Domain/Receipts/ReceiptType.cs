using DotResult;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts;

public readonly record struct ReceiptType(
  ReceiptIdType Id,
  StoreType Store,
  NonFutureDateType PurchaseDate,
  PurchaseIdsType PurchaseIds,
  bool Deleted,
  UnsavedChangesType UnsavedChanges);

public static class Receipt
{
  public static ReceiptType Create(
    StoreType store,
    NonFutureDateType purchaseDate,
    PurchaseIdsType purchaseIds)
  {
    ReceiptIdType receiptId = ReceiptId.Unique();
    return new ReceiptType(
      receiptId,
      store,
      purchaseDate,
      purchaseIds,
      false,
      UnsavedChanges.New(ReceiptCreated.Create(receiptId, store, purchaseDate)));
  }

  public static Result<ReceiptType> ChangeStore(
    this ReceiptType receipt,
    StoreType newStore)
  {
    if (receipt.Deleted)
    {
      return Failure.Validation(message: "Cannot change store of deleted receipt");
    }

    return receipt.Store == newStore
      ? receipt
      : receipt with { Store = newStore, UnsavedChanges = receipt.UnsavedChanges.Append(ReceiptStoreChanged.Create(receipt.Id, newStore)) };
  }

  public static Result<ReceiptType> ChangePurchaseDate(
    this ReceiptType receipt,
    NonFutureDateType newPurchaseDate)
  {
    if (receipt.Deleted)
    {
      return Failure.Validation(message: "Cannot change purchase date of deleted receipt");
    }

    return receipt.PurchaseDate == newPurchaseDate
      ? receipt
      : receipt with { PurchaseDate = newPurchaseDate, UnsavedChanges = receipt.UnsavedChanges.Append(ReceiptPurchaseDateChanged.Create(receipt.Id, newPurchaseDate)) };
  }

  public static Result<ReceiptType> Delete(
    this ReceiptType receipt)
  {
    if (receipt.Deleted)
    {
      return Failure.Validation(message: "Cannot delete already deleted receipt");
    }

    return receipt with { Deleted = true, UnsavedChanges = receipt.UnsavedChanges.Append(ReceiptDeleted.Create(receipt.Id)) };
  }

  public static Result<ReceiptType> AddPurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
  {
    if (receipt.Deleted)
    {
      return Failure.Validation(message: "Cannot add purchase to deleted receipt");
    }

    return receipt.PurchaseIds.Contains(purchaseId)
      ? receipt
      : receipt with { PurchaseIds = receipt.PurchaseIds.Append(purchaseId), UnsavedChanges = receipt.UnsavedChanges.Append(ReceiptPurchaseAdded.Create(receipt.Id, purchaseId)) };
  }

  public static Result<ReceiptType> RemovePurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
  {
    if (receipt.Deleted)
    {
      return Failure.Validation(message: "Cannot remove purchase from deleted receipt");
    }

    return receipt.PurchaseIds.Contains(purchaseId)
      ? receipt with { PurchaseIds = receipt.PurchaseIds.Without(purchaseId), UnsavedChanges = receipt.UnsavedChanges.Append(ReceiptPurchaseRemoved.Create(receipt.Id, purchaseId)) }
      : receipt;
  }
}
