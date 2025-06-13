using DotResult;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetReceiptByIdQuery(Guid Id);

public sealed class GetReceiptByIdHandler(IReceiptRepository receiptRepository)
{
    public async Task<Result<ReceiptDetails>> HandleAsync(
        GetReceiptByIdQuery query,
        CancellationToken cancellationToken) =>
        await ReceiptId.TryCreate(query.Id)
            .ToResult(onNone: () => Failure.Validation(nameof(query.Id), "Invalid receipt id"))
            .MapAsync(id => receiptRepository.GetReceiptByIdAsync(id, cancellationToken))
            .BindAsync(receipt =>
                receipt.ToResult(onNone: () => Failure.Validation(nameof(query.Id), "Invalid receipt id")));
}