namespace ExpenseExplorer.API.Contract.ReadModel;

public record ReceiptHeaderResponse(string Id, string Store, DateOnly PurchaseDate, decimal Total);
