namespace ExpenseExplorer.Application.Features.Receipts.UpdateHeader;

internal static class ErrorCodes
{
    public const string InvalidReceiptId = "INVALID_RECEIPT_ID";
    public const string EmptyStoreName = "EMPTY_STORE_NAME";
    public const string PurchaseDateInFuture = "PURCHASE_DATE_IN_FUTURE";
    public const string InvalidPurchaseDate = "INVALID_PURCHASE_DATE";
}
