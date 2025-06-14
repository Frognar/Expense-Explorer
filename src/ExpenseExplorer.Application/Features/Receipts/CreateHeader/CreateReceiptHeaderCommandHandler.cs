using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

public sealed class CreateReceiptHeaderCommandHandler(
    ICreateReceiptHeaderPersistence persistence)
    : ICommandHandler<CreateReceiptHeaderCommand, CreateReceiptHeaderResponse>
{
    public Task<Result<CreateReceiptHeaderResponse>> HandleAsync(CreateReceiptHeaderCommand command, CancellationToken cancellationToken)
    {
        return persistence.SaveNewReceiptHeaderAsync(
                command.Id,
                command.Store,
                command.PurchaseDate,
                cancellationToken)
            .MapAsync(_ => new CreateReceiptHeaderResponse(command.Id.Value));
    }
}