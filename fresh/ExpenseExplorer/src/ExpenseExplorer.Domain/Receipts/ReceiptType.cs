using DotResult;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.Receipts;

public readonly record struct ReceiptType(
  ReceiptIdType Id,
  StoreType Store,
  NonFutureDateType PurchaseDate,
  PurchaseIdsType PurchaseIds,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version);

public static class Receipt
{
  public static ReceiptType Create(
    StoreType store,
    NonFutureDateType purchaseDate,
    PurchaseIdsType purchaseIds)
  {
    ReceiptIdType receiptId = ReceiptId.Unique();
    Fact fact = ReceiptCreated.Create(receiptId, store, purchaseDate);
    return new ReceiptType(
      receiptId,
      store,
      purchaseDate,
      purchaseIds,
      false,
      UnsavedChanges.New(fact),
      Version.New());
  }

  public static Result<ReceiptType> ChangeStore(
    this ReceiptType receipt,
    StoreType store)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change store of deleted receipt"),
      { } when receipt.Store == store => receipt,
      _ => receipt with
      {
        Store = store, UnsavedChanges = receipt.UnsavedChanges.Append(ReceiptStoreChanged.Create(receipt.Id, store)),
      },
    };

  public static Result<ReceiptType> ChangePurchaseDate(
    this ReceiptType receipt,
    NonFutureDateType purchaseDate)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change purchase date of deleted receipt"),
      { } when receipt.PurchaseDate == purchaseDate => receipt,
      _ => receipt with
      {
        PurchaseDate = purchaseDate,
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptPurchaseDateChanged.Create(receipt.Id, purchaseDate)),
      },
    };

  public static Result<ReceiptType> Delete(
    this ReceiptType receipt)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot delete already deleted receipt"),
      _ => receipt with
      {
        Deleted = true,
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptDeleted.Create(receipt.Id)),
      },
    };

  public static Result<ReceiptType> AddPurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot add purchase to deleted receipt"),
      { } when receipt.PurchaseIds.Contains(purchaseId) => receipt,
      _ => receipt with
      {
        PurchaseIds = receipt.PurchaseIds.Append(purchaseId),
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptPurchaseAdded.Create(receipt.Id, purchaseId)),
      },
    };

  public static Result<ReceiptType> RemovePurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot remove purchase from deleted receipt"),
      { } when !receipt.PurchaseIds.Contains(purchaseId) => receipt,
      _ => receipt with
      {
        PurchaseIds = receipt.PurchaseIds.Without(purchaseId),
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptPurchaseRemoved.Create(receipt.Id, purchaseId)),
      },
    };

  public static Result<ReceiptType> ClearChanges(
    this ReceiptType receipt)
    => receipt.Deleted switch
    {
      true => Failure.Validation(message: "Cannot clear changes of deleted receipt"),
      _ => receipt with { UnsavedChanges = UnsavedChanges.Empty() },
    };

  public static Result<ReceiptType> Recreate(
    IEnumerable<Fact> facts)
    => facts.ToList() switch
    {
      [ReceiptCreated created] => Apply(created),
      [ReceiptCreated created, .. var rest] => rest.Aggregate(Apply(created), ApplyFact),
      _ => Failure.Validation(message: "Invalid receipt facts"),
    };

  private static Result<ReceiptType> ApplyFact(
    this Result<ReceiptType> receipt,
    Fact fact)
    => receipt.Bind(r => r.ApplyFact(fact));

  private static Result<ReceiptType> ApplyFact(
    this ReceiptType receipt,
    Fact fact)
    => fact switch
    {
      ReceiptStoreChanged storeChanged => receipt.Apply(storeChanged),
      ReceiptPurchaseDateChanged purchaseDateChanged => receipt.Apply(purchaseDateChanged),
      ReceiptPurchaseAdded purchaseAdded => receipt.Apply(purchaseAdded),
      ReceiptPurchaseRemoved purchaseRemoved => receipt.Apply(purchaseRemoved),
      ReceiptDeleted => Failure.Validation(message: "Receipt has been deleted"),
      _ => Failure.Validation(message: "Invalid receipt fact"),
    };

  private static Result<ReceiptType> Apply(
    ReceiptCreated fact)
    => (
      from id in ReceiptId.Create(fact.ReceiptId)
      from store in Store.Create(fact.Store)
      from purchaseDate in NonFutureDate.Create(fact.PurchaseDate, fact.CreatedDate)
      select new ReceiptType(
        id,
        store,
        purchaseDate,
        PurchaseIds.New(),
        false,
        UnsavedChanges.Empty(),
        Version.New()))
      .ToResult(() => Failure.Validation(message: "Failed to create receipt"));

  private static Result<ReceiptType> Apply(
    this ReceiptType receipt,
    ReceiptStoreChanged fact)
    => (
        from store in Store.Create(fact.Store)
        select receipt with { Store = store })
      .ToResult(() => Failure.Validation(message: "Failed to change store"));

  private static Result<ReceiptType> Apply(
    this ReceiptType receipt,
    ReceiptPurchaseDateChanged fact)
    => (
        from purchaseDate in NonFutureDate.Create(fact.PurchaseDate, fact.CreatedDate)
        select receipt with { PurchaseDate = purchaseDate })
      .ToResult(() => Failure.Validation(message: "Failed to change purchase date"));

  private static Result<ReceiptType> Apply(
    this ReceiptType receipt,
    ReceiptPurchaseAdded fact)
    => (
        from purchaseId in PurchaseId.Create(fact.PurchaseId)
        select receipt with { PurchaseIds = receipt.PurchaseIds.Append(purchaseId) })
      .ToResult(() => Failure.Validation(message: "Failed to add purchase"));

  private static Result<ReceiptType> Apply(
    this ReceiptType receipt,
    ReceiptPurchaseRemoved fact)
    => (
        from purchaseId in PurchaseId.Create(fact.PurchaseId)
        select receipt with { PurchaseIds = receipt.PurchaseIds.Without(purchaseId) })
      .ToResult(() => Failure.Validation(message: "Failed to remove purchase"));
}
