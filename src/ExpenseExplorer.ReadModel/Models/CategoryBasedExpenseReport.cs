namespace ExpenseExplorer.ReadModel.Models;

public sealed record CategoryBasedExpenseReport(
  DateOnly StartDate,
  DateOnly EndDate,
  decimal Total,
  IEnumerable<ReportEntry> Categories);
