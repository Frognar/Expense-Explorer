namespace ExpenseExplorer.ReadModel.Facts;

public record OpenNewReceiptFact(string Id, string Store, DateOnly PurchaseDate, DateOnly CreatedDate);
