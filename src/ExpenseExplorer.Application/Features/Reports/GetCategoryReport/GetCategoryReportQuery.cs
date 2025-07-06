using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Reports.GetCategoryReport;

public sealed record GetCategoryReportQuery(DateOnly From, DateOnly To)
    : IQuery<GetCategoryReportResponse>;