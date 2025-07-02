using DotResult;
using DotValid;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

internal sealed class CreateReceiptHeaderCommandValidator(
    ICommandHandler<CreateReceiptHeaderCommand, CreateReceiptHeaderResponse> inner)
    : ICommandHandler<CreateReceiptHeaderRequest, CreateReceiptHeaderResponse>
{
    public Task<Result<CreateReceiptHeaderResponse>> HandleAsync(
        CreateReceiptHeaderRequest command,
        CancellationToken cancellationToken)
    {
        return CreateReceiptHeaderCommandFactory.Create
            .Apply(ValidateStore(command.StoreName))
            .Apply(ValidatePurchaseDate(command.PurchaseDate, command.Today)).ToResult()
            .BindAsync(cmd => inner.HandleAsync(cmd, cancellationToken));
    }

    private static Validated<Store> ValidateStore(string storeName) =>
        Store.TryCreate(storeName)
            .ToValidated(() => new ValidationError(nameof(storeName), "Store name cannot be empty"));

    private static Validated<PurchaseDate> ValidatePurchaseDate(DateOnly purchaseDate, DateOnly today) =>
        PurchaseDate.TryCreate(purchaseDate, today)
            .ToValidated(() => new ValidationError(nameof(purchaseDate), "Purchase date cannot be in the future"));
}