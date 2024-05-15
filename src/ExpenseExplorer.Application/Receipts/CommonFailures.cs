namespace ExpenseExplorer.Application.Receipts;

using FunctionalCore.Failures;

internal static class CommonFailures
{
  public static readonly Failure InvalidReceiptId
    = Failure.Validation("ReceiptId", "INVALID_RECEIPT_ID");

  public static readonly Failure InvalidPurchaseId
    = Failure.Validation("PurchaseId", "INVALID_PURCHASE_ID");

  public static readonly IEnumerable<ValidationError> EmptyStoreName
    = [ValidationError.Create("StoreName", "EMPTY_STORE_NAME")];

  public static readonly IEnumerable<ValidationError> FuturePurchaseDate
    = [ValidationError.Create("PurchaseDate", "DATE_IN_FUTURE")];

  public static readonly IEnumerable<ValidationError> EmptyItemName
    = [ValidationError.Create("Item", "EMPTY_ITEM_NAME")];

  public static readonly IEnumerable<ValidationError> EmptyCategory
    = [ValidationError.Create("Category", "EMPTY_CATEGORY")];

  public static readonly IEnumerable<ValidationError> NonPositiveQuantity
    = [ValidationError.Create("Quantity", "NON_POSITIVE_QUANTITY")];

  public static readonly IEnumerable<ValidationError> NegativeUnitPrice
    = [ValidationError.Create("UnitPrice", "NEGATIVE_UNIT_PRICE")];

  public static readonly IEnumerable<ValidationError> NegativeTotalDiscount
    = [ValidationError.Create("TotalDiscount", "NEGATIVE_TOTAL_DISCOUNT")];

  public static readonly IEnumerable<ValidationError> InvalidDescription
    = [ValidationError.Create("Description", "INVALID_DESCRIPTION")];

  public static readonly IEnumerable<ValidationError> EmptySource
    = [ValidationError.Create("Source", "EMPTY_SOURCE_NAME")];

  public static readonly IEnumerable<ValidationError> NegativeAmount
    = [ValidationError.Create("Amount", "NEGATIVE_AMOUNT")];

  public static readonly IEnumerable<ValidationError> FutureReceivedDate
    = [ValidationError.Create("ReceivedDate", "DATE_IN_FUTURE")];
}
