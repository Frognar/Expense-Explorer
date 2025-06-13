using DotResult;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record DeleteReceiptCommand(Guid Id);

public sealed class DeleteReceiptHandler(IReceiptCommandRepository receiptRepository)
{
    public async Task<Result<Unit>> HandleAsync(
        DeleteReceiptCommand command,
        CancellationToken cancellationToken) =>
        await ReceiptId.TryCreate(command.Id)
            .ToResult(onNone: () => Failure.Validation(nameof(command.Id), "Invalid receipt id"))
            .BindAsync(id => receiptRepository.DeleteReceipt(id, cancellationToken));
}