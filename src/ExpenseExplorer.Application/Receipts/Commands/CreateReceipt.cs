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
        Option<Store> store = Store.TryCreate(command.StoreName);

        Validated<Store> validatedStore = store.Match(
            none: () => Validation.Failed<Store>([new ValidationError(nameof(command.StoreName),"Store name cannot be empty")]),
            some: Validation.Succeed);

        Option<PurchaseDate> purchaseDate = PurchaseDate.TryCreate(command.PurchaseDate, command.Today);
        Validated<PurchaseDate> validatedPurchaseDate = purchaseDate.Match(
            none: () => Validation.Failed<PurchaseDate>([new ValidationError(nameof(command.PurchaseDate), "Purchase date cannot be in the future")]),
            some: Validation.Succeed);

        return validatedStore.Match(
            errors: storeErrors => validatedPurchaseDate.Match(
                errors: purchaseDateErrors => Validation.Failed<CreateReceiptRequest>(storeErrors.Concat(purchaseDateErrors)),
                value: _ => Validation.Failed<CreateReceiptRequest>(storeErrors)),
            value: s => validatedPurchaseDate.Match(
                errors: Validation.Failed<CreateReceiptRequest>,
                value: pd => Validation.Succeed(new CreateReceiptRequest(s, pd))));
    }
}