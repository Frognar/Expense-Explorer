using DotResult;
using DotValid;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.Duplicate;

internal sealed class DuplicateReceiptCommandValidator(
    ICommandHandler<DuplicateReceiptCommand, DuplicateReceiptResponse> inner)
    : ICommandHandler<DuplicateReceiptRequest, DuplicateReceiptResponse>
{
    public Task<Result<DuplicateReceiptResponse>> HandleAsync(DuplicateReceiptRequest command,
        CancellationToken cancellationToken)
    {
        return DuplicateReceiptCommandFactory.Create
            .Apply(ValidateReceiptId(command.ReceiptId))
            .Apply(ValidatePurchaseDate(command.Today, command.Today))
            .ToResult()
            .BindAsync(cmd => inner.HandleAsync(cmd, cancellationToken));
    }

    private static Validated<ReceiptId> ValidateReceiptId(Guid receiptId) =>
        ReceiptId.TryCreate(receiptId)
            .ToValidated(() => new ValidationError(nameof(receiptId), ErrorCodes.InvalidReceiptId));

    private static Validated<PurchaseDate> ValidatePurchaseDate(DateOnly purchaseDate, DateOnly today) =>
        purchaseDate == DateOnly.MinValue
            ? Validation.Failed<PurchaseDate>([
                new ValidationError(nameof(purchaseDate), ErrorCodes.InvalidPurchaseDate)
            ])
            : PurchaseDate.TryCreate(purchaseDate, today)
                .ToValidated(() => new ValidationError(nameof(purchaseDate), ErrorCodes.PurchaseDateInFuture));
}