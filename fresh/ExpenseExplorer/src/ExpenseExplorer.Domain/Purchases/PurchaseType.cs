using DotResult;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Purchases.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.Purchases;

public sealed record PurchaseType(
  PurchaseIdType Id,
  ReceiptIdType ReceiptId,
  ItemType Item,
  ExpenseCategoryIdType CategoryId,
  QuantityType Quantity,
  MoneyType UnitPrice,
  MoneyType TotalDiscount,
  DescriptionType Description,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version)
  : EntityType(UnsavedChanges, Version);

public static class Purchase
{
  public static PurchaseType Create(
    ReceiptIdType receiptId,
    ItemType item,
    ExpenseCategoryIdType categoryId,
    QuantityType quantity,
    MoneyType unitPrice,
    MoneyType totalDiscount,
    DescriptionType description)
  {
    PurchaseIdType purchaseId = PurchaseId.Unique();
    Fact fact = PurchaseCreated.Create(
      purchaseId,
      receiptId,
      item,
      categoryId,
      quantity,
      unitPrice,
      totalDiscount,
      description);

    return new PurchaseType(
      purchaseId,
      receiptId,
      item,
      categoryId,
      quantity,
      unitPrice,
      totalDiscount,
      description,
      false,
      UnsavedChanges.New(fact),
      Version.New());
  }

  public static Result<PurchaseType> ChangeItem(
    this PurchaseType purchase,
    ItemType item)
    => purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change item of deleted purchase"),
      not null when purchase.Item == item => purchase,
      not null => purchase with
      {
        Item = item,
        UnsavedChanges = purchase.UnsavedChanges
          .Append(PurchaseItemChanged.Create(purchase.Id, item)),
      },
      _ => Failure.Fatal(message: "Purchase is null"),
    };

  public static Result<PurchaseType> ChangeCategoryId(
    this PurchaseType purchase,
    ExpenseCategoryIdType categoryId)
    => purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change category of deleted purchase"),
      not null when purchase.CategoryId == categoryId => purchase,
      not null => purchase with
      {
        CategoryId = categoryId,
        UnsavedChanges = purchase.UnsavedChanges
          .Append(PurchaseCategoryIdChanged.Create(purchase.Id, categoryId)),
      },
      _ => Failure.Fatal(message: "Purchase is null"),
    };

  public static Result<PurchaseType> ChangeQuantity(
    this PurchaseType purchase,
    QuantityType quantity)
    => purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change quantity of deleted purchase"),
      not null when purchase.Quantity == quantity => purchase,
      not null => purchase with
      {
        Quantity = quantity,
        UnsavedChanges = purchase.UnsavedChanges
          .Append(PurchaseQuantityChanged.Create(purchase.Id, quantity)),
      },
      _ => Failure.Fatal(message: "Purchase is null"),
    };

  public static Result<PurchaseType> ChangeUnitPrice(
    this PurchaseType purchase,
    MoneyType unitPrice)
    => purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change unit price of deleted purchase"),
      not null when purchase.UnitPrice == unitPrice => purchase,
      not null => purchase with
      {
        UnitPrice = unitPrice,
        UnsavedChanges = purchase.UnsavedChanges
          .Append(PurchaseUnitPriceChanged.Create(purchase.Id, unitPrice)),
      },
      _ => Failure.Fatal(message: "Purchase is null"),
    };

  public static Result<PurchaseType> ChangeTotalDiscount(
    this PurchaseType purchase,
    MoneyType totalDiscount)
  {
    return purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change total discount of deleted purchase"),
      not null when purchase.TotalDiscount == totalDiscount => purchase,
      not null => purchase with
      {
        TotalDiscount = totalDiscount,
        UnsavedChanges = purchase.UnsavedChanges
          .Append(PurchaseTotalDiscountChanged.Create(purchase.Id, totalDiscount)),
      },
      _ => Failure.Fatal(message: "Purchase is null"),
    };
  }

  public static Result<PurchaseType> ChangeDescription(
    this PurchaseType purchase,
    DescriptionType description)
    => purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change description of deleted purchase"),
      not null when purchase.Description == description => purchase,
      not null => purchase with
      {
        Description = description,
        UnsavedChanges = purchase.UnsavedChanges
          .Append(PurchaseDescriptionChanged.Create(purchase.Id, description)),
      },
      _ => Failure.Fatal(message: "Purchase is null"),
    };

  public static Result<PurchaseType> Delete(
    this PurchaseType purchase)
    => purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot delete already deleted purchase"),
      not null => purchase with
      {
        Deleted = true,
        UnsavedChanges = purchase.UnsavedChanges
          .Append(PurchaseDeleted.Create(purchase.Id)),
      },
      _ => Failure.Fatal(message: "Purchase is null"),
    };

  public static Result<PurchaseType> ClearChanges(
    this PurchaseType purchase)
    => purchase switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot clear changes of deleted purchase"),
      not null => purchase with { UnsavedChanges = UnsavedChanges.Empty() },
      _ => Failure.Fatal(message: "Purchase is null"),
    };

  public static Result<PurchaseType> Recreate(
    IEnumerable<Fact> facts)
    => facts.ToList() switch
    {
      [PurchaseCreated created] => Apply(created),
      [PurchaseCreated created, .. var rest] => rest.Aggregate(Apply(created), ApplyFact),
      _ => Failure.Validation(message: "Invalid purchase facts"),
    };

  private static Result<PurchaseType> ApplyFact(
    this Result<PurchaseType> purchase,
    Fact fact)
    => purchase.Bind(r => r.ApplyFact(fact));

  private static Result<PurchaseType> ApplyFact(
    this PurchaseType purchase,
    Fact fact)
    => fact switch
    {
      PurchaseItemChanged itemChanged => purchase.Apply(itemChanged),
      PurchaseCategoryIdChanged categoryIdChanged => purchase.Apply(categoryIdChanged),
      PurchaseQuantityChanged quantityChanged => purchase.Apply(quantityChanged),
      PurchaseUnitPriceChanged unitPriceChanged => purchase.Apply(unitPriceChanged),
      PurchaseTotalDiscountChanged totalDiscountChanged => purchase.Apply(totalDiscountChanged),
      PurchaseDescriptionChanged descriptionChanged => purchase.Apply(descriptionChanged),
      PurchaseDeleted => Failure.Validation(message: "Purchase has been deleted"),
      _ => Failure.Validation(message: "Invalid purchase fact"),
    };

  private static Result<PurchaseType> Apply(
    PurchaseCreated fact)
    => (
        from id in PurchaseId.Create(fact.PurchaseId)
        from receiptId in ReceiptId.Create(fact.ReceiptId)
        from item in Item.Create(fact.Item)
        from categoryId in ExpenseCategoryId.Create(fact.CategoryId)
        from quantity in Quantity.Create(fact.Quantity)
        from unitPrice in Money.Create(fact.UnitPriceAmount, fact.UnitPriceCurrency)
        from totalDiscount in Money.Create(fact.TotalDiscountAmount, fact.TotalDiscountCurrency)
        let description = Description.Create(fact.Description)
        select new PurchaseType(
          id,
          receiptId,
          item,
          categoryId,
          quantity,
          unitPrice,
          totalDiscount,
          description,
          false,
          UnsavedChanges.Empty(),
          Version.New()))
      .ToResult(() => Failure.Validation(message: "Failed to create purchase"));

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseItemChanged fact)
    => (
        from item in Item.Create(fact.Item)
        select purchase with { Item = item })
      .ToResult(() => Failure.Validation(message: "Failed to change item"));

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseCategoryIdChanged fact)
    => (
        from categoryId in ExpenseCategoryId.Create(fact.CategoryId)
        select purchase with { CategoryId = categoryId })
      .ToResult(() => Failure.Validation(message: "Failed to change category id"));

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseQuantityChanged fact)
    => (
        from quantity in Quantity.Create(fact.Quantity)
        select purchase with { Quantity = quantity })
      .ToResult(() => Failure.Validation(message: "Failed to change quantity"));

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseUnitPriceChanged fact)
    => (
        from unitPrice in Money.Create(fact.Amount, fact.Currency)
        select purchase with { UnitPrice = unitPrice })
      .ToResult(() => Failure.Validation(message: "Failed to change unit price"));

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseTotalDiscountChanged fact)
    => (
        from totalDiscount in Money.Create(fact.Amount, fact.Currency)
        select purchase with { TotalDiscount = totalDiscount })
      .ToResult(() => Failure.Validation(message: "Failed to change total discount"));

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseDescriptionChanged fact)
    => purchase with { Description = Description.Create(fact.Description) };
}
