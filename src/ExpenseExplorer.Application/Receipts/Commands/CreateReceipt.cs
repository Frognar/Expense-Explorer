using DotResult;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.ValueObjects;
using static ExpenseExplorer.Application.Receipts.Commands.CreateReceiptValidator;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record CreateReceiptCommand(string StoreName, DateOnly PurchaseDate, DateOnly Today);

public sealed class CreateReceiptHandler(IReceiptCommandRepository receiptRepository)
{
    public async Task<Result<ReceiptId>> HandleAsync(
        CreateReceiptCommand command,
        CancellationToken cancellationToken)
    {
        return await Validate(command)
            .ToResult()
            .BindAsync(async request =>
                await receiptRepository.CreateReceipt(request, cancellationToken).MapAsync(_ => request.Id));
    }
}

internal static class CreateReceiptValidator
{
    public static Validated<CreateReceiptRequest> Validate(CreateReceiptCommand command) =>
        CreateReceiptRequest.Create
            .Apply(ValidateStore(command.StoreName))
            .Apply(ValidatePurchaseDate(command.PurchaseDate, command.Today));

    private static Validated<Store> ValidateStore(string storeName) =>
        Store.TryCreate(storeName)
            .ToValidated(() => new ValidationError(nameof(storeName), "Store name cannot be empty"));

    private static Validated<PurchaseDate> ValidatePurchaseDate(DateOnly purchaseDate, DateOnly today) =>
        PurchaseDate.TryCreate(purchaseDate, today)
            .ToValidated(() => new ValidationError(nameof(purchaseDate), "Purchase date cannot be in the future"));
}