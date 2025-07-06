namespace ExpenseExplorer.Application.Features.Reports.GetCategoryReport;

public sealed record GetCategoryReportResponse(IEnumerable<CategoryExpense> Categories);
