
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Features.Reports.GetCategoryReport;

namespace ExpenseExplorer.Application.Features.Reports;

public static class ReportsFeatureFactory
{
    public static IQueryHandler<GetCategoryReportQuery, GetCategoryReportResponse> CreateGetCategoryReportQueryHandler(
        IGetCategoryReportPersistence persistence)
        => new GetCategoryReportQueryHandler(persistence);
}
