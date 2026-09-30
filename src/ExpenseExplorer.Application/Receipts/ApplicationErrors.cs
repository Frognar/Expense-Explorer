using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Application.Receipts;

public static class ApplicationErrors
{
    public static readonly Error ReceiptNotFound = new("Receipt.NotFound", "Receipt was not found.", ErrorType.NotFound);
}
