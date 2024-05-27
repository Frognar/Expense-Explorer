namespace ExpenseExplorer.API.Contract.ReadModel;

public record GenerateCategoryBasedExpenseReportResponse(
  decimal Total,
  IEnumerable<CategoryBasedEntryResponse> Categories);
