using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteHeader;

public sealed class DeleteReceiptHeaderCommandValidator(
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

    private static Validated<ReceiptId> ValidateReceiptId(Guid receiptId)
    {
        return ReceiptId.TryCreate(receiptId)
            .ToValidated(() => new ValidationError(nameof(receiptId), "Receipt ID cannot be empty"));
    }
}