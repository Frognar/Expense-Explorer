using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public sealed class GetReceiptItemsQueryHandler(
    IGetReceiptItemsPersistence persistence)
    : IQueryHandler<GetReceiptItemsQuery, GetReceiptItemsResponse>
{
    public async Task<Result<GetReceiptItemsResponse>> HandleAsync(
        GetReceiptItemsQuery query,
        CancellationToken cancellationToken)
    {
        return await (
            from receipts in persistence
                .GetReceiptItemsAsync(query.PageSize, query.Skip, query.Order, query.Filters, cancellationToken)
            from total in persistence
                .GetTotalCostAsync(query.Filters, cancellationToken)
            select new GetReceiptItemsResponse(receipts, total));
    }
}