namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetIncomesResponse(
  IEnumerable<IncomeResponse> Incomes,
  int TotalCount,
  int FilteredCount,
  int PageSize,
  int PageNumber,
  int PageCount);
