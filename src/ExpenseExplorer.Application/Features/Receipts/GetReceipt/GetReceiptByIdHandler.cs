using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipt;

internal sealed class GetReceiptByIdHandler(IGetReceiptByIdPersistence persistence)
    : IQueryHandler<GetReceiptByIdQuery, GetReceiptByIdResponse>
{
    public async Task<Result<GetReceiptByIdResponse>> HandleAsync(
        GetReceiptByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(query.ReceiptId, cancellationToken)
            .MapAsync(receipt => new GetReceiptByIdResponse(receipt));
    }
}