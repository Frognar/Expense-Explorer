namespace ExpenseExplorer.ReadModel.Models;

public sealed record CategoryBasedExpenseReport(decimal Total, IEnumerable<ReportEntry> Categories);
