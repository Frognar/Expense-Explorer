namespace ExpenseExplorer.Application.Features.Receipts.UpdateItem;

internal static class ErrorCodes
{
    public const string InvalidReceiptItemId = "INVALID_RECEIPT_ITEM_ID";
    public const string InvalidReceiptId = "INVALID_RECEIPT_ID";
    public const string EmptyItemName = "EMPTY_ITEM_NAME";
    public const string EmptyCategoryName = "EMPTY_CATEGORY_NAME";
    public const string InvalidQuantity = "INVALID_QUANTITY";
    public const string InvalidUnitPrice = "INVALID_UNIT_PRICE";
    public const string InvalidDiscount = "INVALID_DISCOUNT";
}