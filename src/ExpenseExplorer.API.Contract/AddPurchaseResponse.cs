namespace ExpenseExplorer.API.Contract;

public record AddPurchaseResponse(
  string Id,
  string StoreName,
  DateOnly PurchaseDate,
  IEnumerable<PurchaseResponse> Purchases,
  ulong Version);
