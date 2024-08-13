using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.API.Mappers;

public static class ReportMapper
{
  public static GenerateIncomeToExpenseReportResponse MapToResponse(this IncomeToExportReport report)
  {
    ArgumentNullException.ThrowIfNull(report);
    return new GenerateIncomeToExpenseReportResponse(
      report.StartDate,
      report.EndDate,
      report.TotalIncome,
      report.TotalExpense);
  }

  public static GenerateCategoryBasedExpenseReportResponse MapToResponse(
    this CategoryBasedExpenseReport report)
  {
    ArgumentNullException.ThrowIfNull(report);
    return new GenerateCategoryBasedExpenseReportResponse(
      report.StartDate,
      report.EndDate,
      report.Total,
      report.Categories.Select(MapToResponse));
  }

  private static CategoryBasedEntryResponse MapToResponse(this ReportEntry entry)
  {
    ArgumentNullException.ThrowIfNull(entry);
    return new CategoryBasedEntryResponse(entry.Category, entry.Total);
  }
}
