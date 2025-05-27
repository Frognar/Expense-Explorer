using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetReceiptByIdQuery(Guid Id);

public sealed class GetReceiptByIdHandler(IReceiptRepository receiptRepository)
{
    public async Task<Result<ReceiptDetails, ValidationError>> HandleAsync(
        GetReceiptByIdQuery query,
        CancellationToken cancellationToken) =>
        await ReceiptId.TryCreate(query.Id)
            .ToResult(onNone: () => new ValidationError(nameof(query.Id), "Invalid receipt id"))
            .MapAsync(id => receiptRepository.GetReceiptByIdAsync(id, cancellationToken))
            .MatchAsync(
                error: Result.Failure<ReceiptDetails, ValidationError>,
                value: receipt =>
                    receipt.ToResult(onNone: () => new ValidationError(nameof(query.Id), "Invalid receipt id")));
}