using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.Duplicate;

public sealed class DuplicateReceiptCommandHandler(
    IDuplicateReceiptPersistence persistence)
    : ICommandHandler<DuplicateReceiptCommand, DuplicateReceiptResponse>
{
    public async Task<Result<DuplicateReceiptResponse>> HandleAsync(DuplicateReceiptCommand command, CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(command.ReceiptId, cancellationToken)
            .BindAsync(r => r.Duplicate(command.PurchaseDate))
            .BindAsync(r => persistence.SaveReceiptAsync(r, cancellationToken)
                .MapAsync(_ => r))
            .MapAsync(r => new DuplicateReceiptResponse(r.Id.Value));
    }
}