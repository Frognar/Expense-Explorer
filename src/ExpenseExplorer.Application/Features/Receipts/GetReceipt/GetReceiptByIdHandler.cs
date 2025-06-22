using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipt;

public sealed class GetReceiptByIdHandler(IGetReceiptByIdPersistence persistence)
    : IQueryHandler<GetReceiptByIdQuery, Result<GetReceiptByIdResponse>>
{
    public async Task<Result<Result<GetReceiptByIdResponse>>> HandleAsync(
        GetReceiptByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(query.ReceiptId, cancellationToken)
            .MapAsync(receipt => new GetReceiptByIdResponse(receipt));
    }
}