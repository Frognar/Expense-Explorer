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
        Validated<CreateReceiptRequest> validated = CreateReceiptValidator.Validate(command);
        Result<CreateReceiptRequest, IEnumerable<string>> result = validated.Match(
            errors => Result.Failure<CreateReceiptRequest, IEnumerable<string>>(errors.Select(e => e.Error)),
            value: Result.Success<CreateReceiptRequest, IEnumerable<string>>);

        return await result.MatchAsync<Result<Unit, IEnumerable<string>>>(
            error: Result.Failure<Unit, IEnumerable<string>>,
            value: async request => Result<Unit, IEnumerable<string>>.Success(await receiptRepository.CreateReceipt(request, cancellationToken)));
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