namespace ExpenseExplorer.API.Contract;

public record UpdateReceiptResponse(
  string Id,
  string StoreName,
  DateOnly PurchaseDate,
  ulong Version);
