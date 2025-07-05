using DotResult;
using DotValid;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteHeader;

internal sealed class DeleteReceiptHeaderCommandValidator(
    ICommandHandler<DeleteReceiptHeaderCommand, DeleteReceiptHeaderResponse> inner)
    : ICommandHandler<DeleteReceiptHeaderRequest, DeleteReceiptHeaderResponse>
{
    public async Task<Result<DeleteReceiptHeaderResponse>> HandleAsync(DeleteReceiptHeaderRequest command,
        CancellationToken cancellationToken)
    {
        return await DeleteReceiptHeaderCommandFactory.Create
            .Apply(ValidateReceiptId(command.ReceiptId))
            .ToResult()
            .BindAsync(cmd => inner.HandleAsync(cmd, cancellationToken));
    }

    private static Validated<ReceiptId> ValidateReceiptId(Guid receiptId) =>
        ReceiptId.TryCreate(receiptId)
            .ToValidated(() => new ValidationError(nameof(receiptId), ErrorCodes.InvalidReceiptId));
}