using DotResult;
using DotValid;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateHeader;

internal sealed class UpdateReceiptHeaderCommandValidator(
    ICommandHandler<UpdateReceiptHeaderCommand, UpdateReceiptHeaderResponse> inner)
    : ICommandHandler<UpdateReceiptHeaderRequest, UpdateReceiptHeaderResponse>
{
    public async Task<Result<UpdateReceiptHeaderResponse>> HandleAsync(
        UpdateReceiptHeaderRequest command,
        CancellationToken cancellationToken)
    {
        return await UpdateReceiptHeaderCommandFactory.Create
            .Apply(ValidateReceiptId(command.ReceiptId))
            .Apply(ValidateStore(command.StoreName))
            .Apply(ValidatePurchaseDate(command.PurchaseDate, command.Today))
            .ToResult()
            .BindAsync(cmd => inner.HandleAsync(cmd, cancellationToken));
    }

    private static Validated<ReceiptId> ValidateReceiptId(Guid receiptId)
    {
        return ReceiptId.TryCreate(receiptId)
            .ToValidated(() => new ValidationError(nameof(receiptId), "Receipt ID cannot be empty"));
    }

    private static Validated<Store> ValidateStore(string storeName) =>
        Store.TryCreate(storeName)
            .ToValidated(() => new ValidationError(nameof(storeName), "Store name cannot be empty"));

    private static Validated<PurchaseDate> ValidatePurchaseDate(DateOnly purchaseDate, DateOnly today) =>
        PurchaseDate.TryCreate(purchaseDate, today)
            .ToValidated(() => new ValidationError(nameof(purchaseDate), "Purchase date cannot be in the future"));
}