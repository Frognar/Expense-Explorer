namespace ExpenseExplorer.API.Contract;

public record GetReceiptsResponse(IEnumerable<ReceiptHeaderResponse> Receipts, int TotalCount);
