namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract.ReadModel;

public static class ReportMapper
{
  public static GenerateIncomeToExpenseReportResponse MapToResponse(this ReadModel.Models.IncomeToExportReport report)
  {
    ArgumentNullException.ThrowIfNull(report);
    return new GenerateIncomeToExpenseReportResponse(report.TotalIncome, report.TotalExpense);
  }

  public static GenerateReportResponse MapToResponse(this ReadModel.Models.Report report)
  {
    ArgumentNullException.ThrowIfNull(report);
    return new GenerateReportResponse(report.Total, report.Categories.Select(MapToResponse));
  }

  private static ReportEntryResponse MapToResponse(this ReadModel.Models.ReportEntry entry)
  {
    ArgumentNullException.ThrowIfNull(entry);
    return new ReportEntryResponse(entry.Category, entry.Total);
  }
}
