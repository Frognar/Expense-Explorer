namespace ExpenseExplorer.Application.Receipts;

using FunctionalCore.Failures;

public static class CommonFailures
{
  public static readonly ValidationFailure InvalidReceiptId = Failure.Validation("ReceiptId", "INVALID_RECEIPT_ID");

  public static readonly ValidationFailure EmptyStoreName = Failure.Validation("StoreName", "EMPTY_STORE_NAME");

  public static readonly ValidationFailure FutureDate = Failure.Validation("PurchaseDate", "EMPTY_STORE_NAME");

  public static readonly ValidationFailure EmptyItemName = Failure.Validation("Item", "EMPTY_ITEM_NAME");

  public static readonly ValidationFailure EmptyCategory = Failure.Validation("Category", "EMPTY_CATEGORY");

  public static readonly ValidationFailure
    NonPositiveQuantity = Failure.Validation("Quantity", "NON_POSITIVE_QUANTITY");

  public static readonly ValidationFailure NegativeUnitPrice = Failure.Validation("UnitPrice", "NEGATIVE_UNIT_PRICE");

  public static readonly ValidationFailure NegativeTotalDiscount = Failure.Validation(
    "TotalDiscount",
    "NEGATIVE_TOTAL_DISCOUNT");
}
