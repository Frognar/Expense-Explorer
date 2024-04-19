namespace ExpenseExplorer.API.Contract;

public record GetReceiptResponse(
  string Id,
  string Store,
  DateOnly PurchaseDate,
  decimal Total,
  IEnumerable<GetReceiptPurchaseResponse> Purchases);
