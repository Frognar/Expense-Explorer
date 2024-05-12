namespace ExpenseExplorer.API.Contract.ReadModel;

public record GenerateReportResponse(decimal Total, IEnumerable<ReportEntryResponse> Categories);
