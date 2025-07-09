using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Reports.GetCategoryReport;

public sealed class GetCategoryReportQueryHandler(IGetCategoryReportPersistence persistence)
    : IQueryHandler<GetCategoryReportQuery, GetCategoryReportResponse>
{
    public async Task<Result<GetCategoryReportResponse>> HandleAsync(
        GetCategoryReportQuery query,
        CancellationToken cancellationToken)
    {
        return await persistence.GetCategoryReportAsync(query.From, query.To, cancellationToken)
            .MapAsync(expenses => expenses.OrderByDescending(e => e.Total))
            .MapAsync(expenses => new GetCategoryReportResponse(expenses));
    }
}
