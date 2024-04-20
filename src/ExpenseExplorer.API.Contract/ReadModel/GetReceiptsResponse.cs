namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetReceiptsResponse(
  IEnumerable<ReceiptHeaderResponse> Receipts,
  int TotalCount,
  int PageSize,
  int PageNumber,
  int PageCount);
