using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteItem;

internal sealed class DeleteReceiptItemCommandValidator(
    ICommandHandler<DeleteReceiptItemCommand, DeleteReceiptItemResponse> inner)
    : ICommandHandler<DeleteReceiptItemRequest, DeleteReceiptItemResponse>
{
    public async Task<Result<DeleteReceiptItemResponse>> HandleAsync(
        DeleteReceiptItemRequest command,
        CancellationToken cancellationToken)
    {
        return await DeleteReceiptItemCommandFactory.Create
            .Apply(ValidateReceiptId(command.ReceiptId))
            .Apply(ValidateReceiptItemId(command.ReceiptItemId))
            .ToResult()
            .BindAsync(cmd => inner.HandleAsync(cmd, cancellationToken));
    }

    private static Validated<ReceiptId> ValidateReceiptId(Guid receiptId)
    {
        return ReceiptId.TryCreate(receiptId)
            .ToValidated(() => new ValidationError(nameof(receiptId), "Receipt ID cannot be empty"));
    }

    private static Validated<ReceiptItemId> ValidateReceiptItemId(Guid receiptItemId)
    {
        return ReceiptItemId.TryCreate(receiptItemId)
            .ToValidated(() => new ValidationError(nameof(receiptItemId), "Receipt Item ID cannot be empty"));
    }
}