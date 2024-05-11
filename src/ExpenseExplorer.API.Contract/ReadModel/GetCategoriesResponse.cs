namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetCategoriesResponse(
  IEnumerable<string> Categories,
  int TotalCount,
  int PageSize,
  int PageNumber,
  int PageCount);
