namespace ExpenseExplorer.ReadModel.Models;

public sealed record Report(decimal Total, IEnumerable<ReportEntry> Categories);
