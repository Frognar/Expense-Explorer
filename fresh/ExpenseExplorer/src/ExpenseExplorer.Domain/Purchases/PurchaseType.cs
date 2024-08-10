using ExpenseExplorer.Domain.Purchases.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases;

public readonly record struct PurchaseType(
  PurchaseIdType Id,
  ReceiptIdType ReceiptId,
  ItemType Item,
  ExpenseCategoryIdType CategoryId,
  QuantityType Quantity,
  MoneyType UnitPrice,
  MoneyType TotalDiscount,
  DescriptionType Description,
  bool Deleted,
  UnsavedChangesType UnsavedChanges);

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
          description)));
  }

  public static PurchaseType ChangeItem(
    this PurchaseType purchase,
    ItemType newItem)
  {
    return purchase.Item == newItem
      ? purchase
      : purchase with { Item = newItem, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseItemChanged.Create(purchase.Id, newItem)) };
  }

  public static PurchaseType ChangeCategoryId(
    this PurchaseType purchase,
    ExpenseCategoryIdType newCategoryId)
  {
    return purchase.CategoryId == newCategoryId
      ? purchase
      : purchase with { CategoryId = newCategoryId, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseCategoryIdChanged.Create(purchase.Id, newCategoryId)) };
  }

  public static PurchaseType ChangeQuantity(
    this PurchaseType purchase,
    QuantityType newQuantity)
  {
    return purchase.Quantity == newQuantity
      ? purchase
      : purchase with { Quantity = newQuantity, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseQuantityChanged.Create(purchase.Id, newQuantity)) };
  }

  public static PurchaseType ChangeUnitPrice(
    this PurchaseType purchase,
    MoneyType newUnitPrice)
  {
    return purchase.UnitPrice == newUnitPrice
      ? purchase
      : purchase with { UnitPrice = newUnitPrice, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseUnitPriceChanged.Create(purchase.Id, newUnitPrice)) };
  }

  public static PurchaseType ChangeTotalDiscount(
    this PurchaseType purchase,
    MoneyType newTotalDiscount)
  {
    return purchase.TotalDiscount == newTotalDiscount
      ? purchase
      : purchase with { TotalDiscount = newTotalDiscount, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseTotalDiscountChanged.Create(purchase.Id, newTotalDiscount)) };
  }

  public static PurchaseType ChangeDescription(
    this PurchaseType purchase,
    DescriptionType newDescription)
  {
    return purchase.Description == newDescription
      ? purchase
      : purchase with { Description = newDescription, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseDescriptionChanged.Create(purchase.Id, newDescription)) };
  }

  public static PurchaseType Delete(
    this PurchaseType purchase)
  {
    return purchase with { Deleted = true, UnsavedChanges = purchase.UnsavedChanges.Append(PurchaseDeleted.Create(purchase.Id)) };
  }
}
