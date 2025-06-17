using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteItem;

public sealed class DeleteReceiptItemCommandHandler(
    IReceiptItemDeletePersistence persistence)
    : ICommandHandler<DeleteReceiptItemCommand, DeleteReceiptItemResponse>
{
    public async Task<Result<DeleteReceiptItemResponse>> HandleAsync(
        DeleteReceiptItemCommand command,
        CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(command.ReceiptId, cancellationToken)
            .BindAsync(receipt => receipt.RemoveItem(command.ReceiptItemId))
            .BindAsync(receipt => persistence.SaveReceiptAsync(receipt, cancellationToken))
            .MapAsync(_ => new DeleteReceiptItemResponse());
    }
}