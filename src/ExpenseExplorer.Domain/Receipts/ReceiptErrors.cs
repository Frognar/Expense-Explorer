using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public static class ReceiptErrors
{
    public static readonly Error ItemNotFound = new("Receipt.ItemNotFound", "Receipt item was not found.");

    public static readonly Error ItemAlreadyExists = new("Receipt.ItemAlreadyExists", "Receipt already has an item with this identifier.");
}
