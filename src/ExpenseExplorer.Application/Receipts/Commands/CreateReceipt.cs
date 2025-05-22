using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record CreateReceiptCommand(string StoreName, DateOnly PurchaseDate, DateOnly Today);

public sealed class CreateReceiptHandler(IReceiptRepository receiptRepository)
{
    public async Task<Result<Unit, IEnumerable<string>>> HandleAsync(
        CreateReceiptCommand command,
        CancellationToken cancellationToken)
    {
        return await CreateReceiptValidator.Validate(command)
            .ToResult()
            .MapError(errors => errors.Select(e => e.Error))
            .MapAsync(request => receiptRepository.CreateReceipt(request, cancellationToken));
    }
}

public static class CreateReceiptValidator
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