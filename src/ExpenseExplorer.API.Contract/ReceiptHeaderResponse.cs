namespace ExpenseExplorer.API.Contract;

public record ReceiptHeaderResponse(string Id, string Store, DateOnly PurchaseDate, decimal Total);
