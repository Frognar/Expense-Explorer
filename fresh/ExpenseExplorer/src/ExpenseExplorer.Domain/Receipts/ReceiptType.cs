using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.Facts;
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

  public static Result<ReceiptType> ClearChanges(
    this ReceiptType receipt)
  {
    if (receipt.Deleted)
    {
      return Failure.Validation(message: "Cannot clear changes of deleted receipt");
    }

    return receipt with { UnsavedChanges = UnsavedChanges.Empty() };
  }

  public static Result<ReceiptType> Recreate(IEnumerable<Fact> facts)
  {
    facts = facts.ToList();
    if (facts.FirstOrDefault() is ReceiptCreated receiptCreated)
    {
      return facts.Skip(1)
        .Aggregate(
          Apply(receiptCreated),
          (receipt, fact) => receipt.Bind(r => r.ApplyFact(fact)));
    }

    return Failure.Validation(message: "Invalid receipt facts");
  }

  private static Result<ReceiptType> ApplyFact(this ReceiptType receipt, Fact fact)
  {
    return fact switch
    {
      ReceiptCreated receiptCreated => Apply(receiptCreated),
      ReceiptStoreChanged receiptStoreChanged => receipt.Apply(receiptStoreChanged),
      ReceiptPurchaseDateChanged receiptPurchaseDateChanged => receipt.Apply(receiptPurchaseDateChanged),
      ReceiptPurchaseAdded receiptPurchaseAdded => receipt.Apply(receiptPurchaseAdded),
      ReceiptPurchaseRemoved receiptPurchaseRemoved => receipt.Apply(receiptPurchaseRemoved),
      ReceiptDeleted => Failure.Validation(message: "Receipt has been deleted"),
      _ => Failure.Validation(message: "Invalid receipt fact"),
    };
  }

  private static Result<ReceiptType> Apply(ReceiptCreated fact)
  {
    Maybe<ReceiptType> receipt =
      from id in ReceiptId.Create(fact.ReceiptId)
      from store in Store.Create(fact.Store)
      from purchaseDate in NonFutureDate.Create(fact.PurchaseDate, fact.CreatedDate)
      select new ReceiptType(
        id,
        store,
        purchaseDate,
        PurchaseIds.New(),
        false,
        UnsavedChanges.Empty());

    return receipt.Match(
      () => Failure.Validation(message: "Failed to create receipt"),
      Success.From);
  }

  private static Result<ReceiptType> Apply(this ReceiptType receipt, ReceiptStoreChanged fact)
  {
    return (
        from store in Store.Create(fact.Store)
        select receipt with { Store = store })
      .Match(
        () => Failure.Validation(message: "Failed to change store"),
        Success.From);
  }

  private static Result<ReceiptType> Apply(this ReceiptType receipt, ReceiptPurchaseDateChanged fact)
  {
    return (
        from purchaseDate in NonFutureDate.Create(fact.PurchaseDate, fact.CreatedDate)
        select receipt with { PurchaseDate = purchaseDate })
      .Match(
        () => Failure.Validation(message: "Failed to change purchase date"),
        Success.From);
  }

  private static Result<ReceiptType> Apply(this ReceiptType receipt, ReceiptPurchaseAdded fact)
  {
    return (
        from purchaseId in PurchaseId.Create(fact.PurchaseId)
        select receipt with { PurchaseIds = receipt.PurchaseIds.Append(purchaseId) })
      .Match(
        () => Failure.Validation(message: "Failed to add purchase"),
        Success.From);
  }

  private static Result<ReceiptType> Apply(this ReceiptType receipt, ReceiptPurchaseRemoved fact)
  {
    return (
        from purchaseId in PurchaseId.Create(fact.PurchaseId)
        select receipt with { PurchaseIds = receipt.PurchaseIds.Without(purchaseId) })
      .Match(
        () => Failure.Validation(message: "Failed to remove purchase"),
        Success.From);
  }
}
