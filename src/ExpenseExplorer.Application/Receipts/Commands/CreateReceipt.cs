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
    public static Validated<CreateReceiptRequest> Validate(CreateReceiptCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Validated<Store> validatedStore = Store.TryCreate(command.StoreName)
                .ToValidated(() => new ValidationError(nameof(command.StoreName), "Store name cannot be empty"));

        Validated<PurchaseDate> validatedPurchaseDate = PurchaseDate.TryCreate(command.PurchaseDate, command.Today)
            .ToValidated(() => new ValidationError(nameof(command.PurchaseDate), "Purchase date cannot be in the future"));

        return validatedStore.Join(validatedPurchaseDate, (store, purchaseDate) => new CreateReceiptRequest(store, purchaseDate));
    }
}