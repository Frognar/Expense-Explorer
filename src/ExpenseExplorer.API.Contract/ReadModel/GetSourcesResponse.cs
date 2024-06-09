namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetSourcesResponse(
  IEnumerable<string> Sources,
  int TotalCount,
  int PageSize,
  int PageNumber,
  int PageCount);
