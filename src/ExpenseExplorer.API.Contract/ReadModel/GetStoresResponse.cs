namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetStoresResponse(
  IEnumerable<string> Stores,
  int TotalCount,
  int FilteredCount,
  int PageSize,
  int PageNumber,
  int PageCount);
