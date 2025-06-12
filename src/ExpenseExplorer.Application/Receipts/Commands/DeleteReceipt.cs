using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record DeleteReceiptCommand(Guid Id);

public sealed class DeleteReceiptHandler(IReceiptCommandRepository receiptRepository)
{
    public async Task<Result<Unit, ValidationError>> HandleAsync(
        DeleteReceiptCommand command,
        CancellationToken cancellationToken) =>
        await ReceiptId.TryCreate(command.Id)
            .ToResult(onNone: () => "Invalid receipt id")
            .FlatMapAsync(id => receiptRepository.DeleteReceipt(id ,cancellationToken))
            .MapErrorAsync(error => new ValidationError(nameof(command.Id), error));
}