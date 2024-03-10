namespace ExpenseExplorer.Application.Receipts.Commands;

public record OpenNewReceiptCommand(string StoreName, DateOnly PurchaseDate, DateOnly Today);
