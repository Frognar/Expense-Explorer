namespace ExpenseExplorer.API.Contract.ReadModel;

public record GenerateCategoryBasedExpenseReportResponse(
  DateOnly StartDate,
  DateOnly EndDate,
  decimal Total,
  IEnumerable<CategoryBasedEntryResponse> Categories);
