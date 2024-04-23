namespace ExpenseExplorer.API.Contract;

public record UpdateReceiptRequest(string? StoreName, DateOnly? PurchaseDate);
