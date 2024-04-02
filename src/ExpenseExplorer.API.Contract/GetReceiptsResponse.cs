namespace ExpenseExplorer.API.Contract;

public record GetReceiptsResponse(IEnumerable<ReceiptResponse> Receipts, int TotalCount);
