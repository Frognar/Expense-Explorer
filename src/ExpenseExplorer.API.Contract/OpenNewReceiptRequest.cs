namespace ExpenseExplorer.API.Contract;

public record OpenNewReceiptRequest(string StoreName, DateOnly PurchaseDate);
