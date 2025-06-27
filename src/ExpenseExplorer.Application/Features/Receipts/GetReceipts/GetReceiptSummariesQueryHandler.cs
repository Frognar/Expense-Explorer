using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipts;

internal sealed class GetReceiptSummariesQueryHandler(
    IGetReceiptSummariesPersistence persistence)
    : IQueryHandler<GetReceiptSummariesQuery, GetReceiptSummariesResponse>
{
    public async Task<Result<GetReceiptSummariesResponse>> HandleAsync(
        GetReceiptSummariesQuery query,
        CancellationToken cancellationToken)
    {
        return await (
            from receipts in persistence
                .GetReceiptsAsync(query.PageSize, query.Skip, query.Order, query.Filters, cancellationToken)
            from total in persistence
                .GetTotalCostAsync(query.Filters, cancellationToken)
            select new GetReceiptSummariesResponse(receipts, total));
    }
}