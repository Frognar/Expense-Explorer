using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record DeleteReceiptCommand(Guid Id);

public sealed class DeleteReceiptHandler(IReceiptRepository receiptRepository)
{
    public async Task<Result<Unit, ValidationError>> HandleAsync(
        DeleteReceiptCommand command,
        CancellationToken cancellationToken) =>
        await ReceiptId.TryCreate(command.Id)
            .ToResult(onNone: () => new ValidationError(nameof(command.Id), "Invalid receipt id"))
            .MapAsync(id => receiptRepository.DeleteReceipt(id, cancellationToken));
}