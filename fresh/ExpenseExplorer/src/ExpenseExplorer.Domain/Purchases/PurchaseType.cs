using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Purchases.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.Purchases;

public readonly record struct PurchaseType(PurchaseIdType Id,
  ReceiptIdType ReceiptId,
  ItemType Item,
  ExpenseCategoryIdType CategoryId,
  QuantityType Quantity,
  MoneyType UnitPrice,
  MoneyType TotalDiscount,
  DescriptionType Description,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version);

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
      UnsavedChanges.New(
        PurchaseCreated.Create(
          purchaseId,
          receiptId,
          item,
          categoryId,
          quantity,
          unitPrice,
          totalDiscount,
          description)),
      Version.New());
  }

  public static Result<PurchaseType> ChangeItem(
    this PurchaseType purchase,
    ItemType newItem)
  {
    if (purchase.Deleted)
    {
      return Failure.Validation(message: "Cannot change item of deleted purchase");
    }

    return purchase.Item == newItem
      ? purchase
      : purchase with { Item = newItem, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseItemChanged.Create(purchase.Id, newItem)) };
  }

  public static Result<PurchaseType> ChangeCategoryId(
    this PurchaseType purchase,
    ExpenseCategoryIdType newCategoryId)
  {
    if (purchase.Deleted)
    {
      return Failure.Validation(message: "Cannot change category of deleted purchase");
    }

    return purchase.CategoryId == newCategoryId
      ? purchase
      : purchase with { CategoryId = newCategoryId, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseCategoryIdChanged.Create(purchase.Id, newCategoryId)) };
  }

  public static Result<PurchaseType> ChangeQuantity(
    this PurchaseType purchase,
    QuantityType newQuantity)
  {
    if (purchase.Deleted)
    {
      return Failure.Validation(message: "Cannot change quantity of deleted purchase");
    }

    return purchase.Quantity == newQuantity
      ? purchase
      : purchase with { Quantity = newQuantity, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseQuantityChanged.Create(purchase.Id, newQuantity)) };
  }

  public static Result<PurchaseType> ChangeUnitPrice(
    this PurchaseType purchase,
    MoneyType newUnitPrice)
  {
    if (purchase.Deleted)
    {
      return Failure.Validation(message: "Cannot change unit price of deleted purchase");
    }

    return purchase.UnitPrice == newUnitPrice
      ? purchase
      : purchase with { UnitPrice = newUnitPrice, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseUnitPriceChanged.Create(purchase.Id, newUnitPrice)) };
  }

  public static Result<PurchaseType> ChangeTotalDiscount(
    this PurchaseType purchase,
    MoneyType newTotalDiscount)
  {
    if (purchase.Deleted)
    {
      return Failure.Validation(message: "Cannot change total discount of deleted purchase");
    }

    return purchase.TotalDiscount == newTotalDiscount
      ? purchase
      : purchase with { TotalDiscount = newTotalDiscount, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseTotalDiscountChanged.Create(purchase.Id, newTotalDiscount)) };
  }

  public static Result<PurchaseType> ChangeDescription(
    this PurchaseType purchase,
    DescriptionType newDescription)
  {
    if (purchase.Deleted)
    {
      return Failure.Validation(message: "Cannot change description of deleted purchase");
    }

    return purchase.Description == newDescription
      ? purchase
      : purchase with { Description = newDescription, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseDescriptionChanged.Create(purchase.Id, newDescription)) };
  }

  public static Result<PurchaseType> Delete(this PurchaseType purchase)
  {
    if (purchase.Deleted)
    {
      return Failure.Validation(message: "Cannot delete already deleted purchase");
    }

    return purchase with { Deleted = true, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseDeleted.Create(purchase.Id)) };
  }

  public static Result<PurchaseType> Recreate(IEnumerable<Fact> facts)
  {
    facts = facts.ToList();
    if (facts.FirstOrDefault() is PurchaseCreated purchaseCreated)
    {
      return facts.Skip(1)
        .Aggregate(
          Apply(purchaseCreated),
          (purchase, fact)
            => purchase.Bind(r => r.ApplyFact(fact)));
    }

    return Failure.Validation(message: "Invalid purchase facts");
  }

  private static Result<PurchaseType> ApplyFact(
    this PurchaseType purchase,
    Fact fact)
  {
    return fact switch
    {
      PurchaseItemChanged purchaseItemChanged
        => purchase.Apply(purchaseItemChanged),
      PurchaseCategoryIdChanged purchaseCategoryIdChanged
        => purchase.Apply(purchaseCategoryIdChanged),
      PurchaseQuantityChanged purchaseQuantityChanged
        => purchase.Apply(purchaseQuantityChanged),
      PurchaseUnitPriceChanged purchaseUnitPriceChanged
        => purchase.Apply(purchaseUnitPriceChanged),
      PurchaseTotalDiscountChanged purchaseTotalDiscountChanged
        => purchase.Apply(purchaseTotalDiscountChanged),
      PurchaseDescriptionChanged purchaseDescriptionChanged
        => purchase.Apply(purchaseDescriptionChanged),
      PurchaseDeleted
        => Failure.Validation(message: "Purchase has been deleted"),
      _ => Failure.Validation(message: "Invalid purchase fact"),
    };
  }

  private static Result<PurchaseType> Apply(PurchaseCreated fact)
  {
    Maybe<PurchaseType> purchase =
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
        Version.New());

    return purchase.Match(
      () => Failure.Validation(message: "Failed to create purchase"),
      Success.From);
  }

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseItemChanged fact)
  {
    return (
        from item in Item.Create(fact.Item)
        select purchase with { Item = item })
      .Match(
        () => Failure.Validation(message: "Failed to change item"),
        Success.From);
  }

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseCategoryIdChanged fact)
  {
    return (
        from categoryId in ExpenseCategoryId.Create(fact.CategoryId)
        select purchase with { CategoryId = categoryId })
      .Match(
        () => Failure.Validation(message: "Failed to change category id"),
        Success.From);
  }

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseQuantityChanged fact)
  {
    return (
        from quantity in Quantity.Create(fact.Quantity)
        select purchase with { Quantity = quantity })
      .Match(
        () => Failure.Validation(message: "Failed to change quantity"),
        Success.From);
  }

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseUnitPriceChanged fact)
  {
    return (
        from unitPrice in Money.Create(fact.Amount, fact.Currency)
        select purchase with { UnitPrice = unitPrice })
      .Match(
        () => Failure.Validation(message: "Failed to change unit price"),
        Success.From);
  }

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseTotalDiscountChanged fact)
  {
    return (
        from totalDiscount in Money.Create(fact.Amount, fact.Currency)
        select purchase with { TotalDiscount = totalDiscount })
      .Match(
        () => Failure.Validation(message: "Failed to change total discount"),
        Success.From);
  }

  private static Result<PurchaseType> Apply(
    this PurchaseType purchase,
    PurchaseDescriptionChanged fact)
  {
    return purchase with { Description = Description.Create(fact.Description) };
  }
}
