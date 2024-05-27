namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract.ReadModel;

public static class ReportMapper
{
  public static GenerateIncomeToExpenseReportResponse MapToResponse(this ReadModel.Models.IncomeToExportReport report)
  {
    ArgumentNullException.ThrowIfNull(report);
    return new GenerateIncomeToExpenseReportResponse(
      report.StartDate,
      report.EndDate,
      report.TotalIncome,
      report.TotalExpense);
  }

  public static GenerateCategoryBasedExpenseReportResponse MapToResponse(
    this ReadModel.Models.CategoryBasedExpenseReport report)
  {
    ArgumentNullException.ThrowIfNull(report);
    return new GenerateCategoryBasedExpenseReportResponse(
      report.StartDate,
      report.EndDate,
      report.Total,
      report.Categories.Select(MapToResponse));
  }

  private static CategoryBasedEntryResponse MapToResponse(this ReadModel.Models.ReportEntry entry)
  {
    ArgumentNullException.ThrowIfNull(entry);
    return new CategoryBasedEntryResponse(entry.Category, entry.Total);
  }
}
