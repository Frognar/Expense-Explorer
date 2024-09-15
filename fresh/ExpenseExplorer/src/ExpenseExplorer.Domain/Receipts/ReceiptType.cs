using DotResult;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record ReceiptType(
  ReceiptIdType Id,
  StoreType Store,
  NonFutureDateType PurchaseDate,
  PurchaseIdsType PurchaseIds,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version)
  : EntityType(UnsavedChanges, Version);

public static class Receipt
{
  public static ReceiptType Create(
    StoreType store,
    NonFutureDateType purchaseDate)
  {
    ReceiptIdType receiptId = ReceiptId.Unique();
    Fact fact = ReceiptCreated.Create(receiptId, store, purchaseDate);
    return new ReceiptType(
      receiptId,
      store,
      purchaseDate,
      PurchaseIds.Empty(),
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
      not null when receipt.Store == store => receipt,
      not null => receipt with
      {
        Store = store, UnsavedChanges = receipt.UnsavedChanges.Append(ReceiptStoreChanged.Create(receipt.Id, store)),
      },
      _ => Failure.Fatal(message: "Receipt is null"),
    };

  public static Result<ReceiptType> ChangePurchaseDate(
    this ReceiptType receipt,
    NonFutureDateType purchaseDate)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change purchase date of deleted receipt"),
      not null when receipt.PurchaseDate == purchaseDate => receipt,
      not null => receipt with
      {
        PurchaseDate = purchaseDate,
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptPurchaseDateChanged.Create(receipt.Id, purchaseDate)),
      },
      _ => Failure.Fatal(message: "Receipt is null"),
    };

  public static Result<ReceiptType> Delete(
    this ReceiptType receipt)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot delete already deleted receipt"),
      not null => receipt with
      {
        Deleted = true,
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptDeleted.Create(receipt.Id)),
      },
      _ => Failure.Fatal(message: "Receipt is null"),
    };

  public static Result<ReceiptType> AddPurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot add purchase to deleted receipt"),
      not null when receipt.PurchaseIds.Contains(purchaseId) => receipt,
      not null => receipt with
      {
        PurchaseIds = receipt.PurchaseIds.Append(purchaseId),
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptPurchaseAdded.Create(receipt.Id, purchaseId)),
      },
      _ => Failure.Fatal(message: "Receipt is null"),
    };

  public static Result<ReceiptType> RemovePurchase(
    this ReceiptType receipt,
    PurchaseIdType purchaseId)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot remove purchase from deleted receipt"),
      not null when !receipt.PurchaseIds.Contains(purchaseId) => receipt,
      not null => receipt with
      {
        PurchaseIds = receipt.PurchaseIds.Without(purchaseId),
        UnsavedChanges = receipt.UnsavedChanges
          .Append(ReceiptPurchaseRemoved.Create(receipt.Id, purchaseId)),
      },
      _ => Failure.Fatal(message: "Receipt is null"),
    };

  public static Result<ReceiptType> ClearChanges(
    this ReceiptType receipt)
    => receipt switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot clear changes of deleted receipt"),
      not null => receipt with { UnsavedChanges = UnsavedChanges.Empty() },
      _ => Failure.Fatal(message: "Receipt is null"),
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
