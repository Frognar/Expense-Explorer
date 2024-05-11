namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetItemsResponse(
  IEnumerable<string> Items,
  int TotalCount,
  int PageSize,
  int PageNumber,
  int PageCount);
