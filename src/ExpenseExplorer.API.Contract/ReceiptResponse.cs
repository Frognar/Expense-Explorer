namespace ExpenseExplorer.API.Contract;

public record ReceiptResponse(
  string Id,
  string StoreName,
  DateOnly PurchaseDate,
  IEnumerable<PurchaseResponse> Purchases,
  ulong Version);
