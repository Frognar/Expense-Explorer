using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

public sealed class CreateReceiptHeaderCommandHandler(
    ICreateReceiptHeaderPersistence persistence)
    : ICommandHandler<CreateReceiptHeaderCommand, CreateReceiptHeaderResponse>
{
    public Task<Result<CreateReceiptHeaderResponse>> HandleAsync(CreateReceiptHeaderCommand command, CancellationToken cancellationToken)
    {
        return ReceiptFactory.Create(command.Id, command.Store, command.PurchaseDate)
            .ToResult()
            .BindAsync(r => persistence.SaveNewReceiptHeaderAsync(r, cancellationToken))
            .MapAsync(_ => new CreateReceiptHeaderResponse(command.Id.Value));
    }
}