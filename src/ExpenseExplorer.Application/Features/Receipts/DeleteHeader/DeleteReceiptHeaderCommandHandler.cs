using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteHeader;

internal sealed class DeleteReceiptHeaderCommandHandler(
    IDeleteReceiptHeaderPersistence persistence)
    : ICommandHandler<DeleteReceiptHeaderCommand, DeleteReceiptHeaderResponse>
{
    public async Task<Result<DeleteReceiptHeaderResponse>> HandleAsync(DeleteReceiptHeaderCommand command,
        CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(command.ReceiptId, cancellationToken)
            .BindAsync(r => r.Delete())
            .BindAsync(async r => await persistence.SaveReceiptAsync(r, cancellationToken))
            .MapAsync(_ => new DeleteReceiptHeaderResponse());
    }
}