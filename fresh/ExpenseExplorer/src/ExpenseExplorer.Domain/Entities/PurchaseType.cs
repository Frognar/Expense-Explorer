using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Entities;

public readonly record struct PurchaseType(
  PurchaseIdType Id,
  ReceiptIdType ReceiptId,
  ItemType Item,
  ExpenseCategoryIdType CategoryId,
  QuantityType Quantity,
  MoneyType UnitPrice,
  MoneyType TotalDiscount,
  DescriptionType Description,
  bool Deleted);

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
    return new PurchaseType(
      PurchaseId.Unique(),
      receiptId,
      item,
      categoryId,
      quantity,
      unitPrice,
      totalDiscount,
      description,
      false);
  }

  public static PurchaseType ChangeItem(
    this PurchaseType purchase,
    ItemType newItem)
  {
    return purchase.Item == newItem
      ? purchase
      : purchase with { Item = newItem };
  }

  public static PurchaseType ChangeCategoryId(
    this PurchaseType purchase,
    ExpenseCategoryIdType newCategoryId)
  {
    return purchase.CategoryId == newCategoryId
      ? purchase
      : purchase with { CategoryId = newCategoryId };
  }

  public static PurchaseType ChangeQuantity(
    this PurchaseType purchase,
    QuantityType newQuantity)
  {
    return purchase.Quantity == newQuantity
      ? purchase
      : purchase with { Quantity = newQuantity };
  }

  public static PurchaseType ChangeUnitPrice(
    this PurchaseType purchase,
    MoneyType newUnitPrice)
  {
    return purchase.UnitPrice == newUnitPrice
      ? purchase
      : purchase with { UnitPrice = newUnitPrice };
  }

  public static PurchaseType ChangeTotalDiscount(
    this PurchaseType purchase,
    MoneyType newTotalDiscount)
  {
    return purchase.TotalDiscount == newTotalDiscount
      ? purchase
      : purchase with { TotalDiscount = newTotalDiscount };
  }

  public static PurchaseType ChangeDescription(
    this PurchaseType purchase,
    DescriptionType newDescription)
  {
    return purchase.Description == newDescription
      ? purchase
      : purchase with { Description = newDescription };
  }

  public static PurchaseType Delete(
    this PurchaseType purchase)
  {
    return purchase with { Deleted = true };
  }
}
