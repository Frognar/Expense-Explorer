namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetReceiptResponse(
  string Id,
  string Store,
  DateOnly PurchaseDate,
  decimal Total,
  IEnumerable<GetReceiptPurchaseResponse> Purchases);
