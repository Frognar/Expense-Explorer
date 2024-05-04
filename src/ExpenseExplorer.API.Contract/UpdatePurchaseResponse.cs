namespace ExpenseExplorer.API.Contract;

public record UpdatePurchaseResponse(
  string Id,
  string StoreName,
  DateOnly PurchaseDate,
  IEnumerable<PurchaseResponse> Purchases,
  ulong Version);
