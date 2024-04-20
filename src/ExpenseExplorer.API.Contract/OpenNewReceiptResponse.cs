namespace ExpenseExplorer.API.Contract;

public record OpenNewReceiptResponse(
  string Id,
  string StoreName,
  DateOnly PurchaseDate,
  ulong Version);
