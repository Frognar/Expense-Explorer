using ExpenseExplorer.WebApp.Data;
using ExpenseExplorer.WebApp.Helpers;
using ExpenseExplorer.WebApp.Models;

namespace ExpenseExplorer.WebApp.Services;

internal sealed class ReceiptService(
    IReceiptRepository receiptRepository)
{
    public async Task<ReceiptDetailsPage> GetReceiptsAsync(
        int pageSize,
        int skip,
        string? orderBy,
        IEnumerable<string> stores,
        DateOnly? purchaseDateFrom,
        DateOnly? purchaseDateTo,
        decimal? totalCostMin,
        decimal? totalCostMax)
    {
        return await receiptRepository
            .GetReceiptsAsync(
                pageSize,
                skip,
                orderBy?
                    .Replace(" asc", "", StringComparison.InvariantCultureIgnoreCase)
                    .Replace(" desc", "", StringComparison.InvariantCultureIgnoreCase)
                ?? "",
                orderBy?.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase) == true
                    ? SortDirection.Descending
                    : SortDirection.Ascending,
                stores,
                purchaseDateFrom,
                purchaseDateTo,
                totalCostMin,
                totalCostMax);
    }

    internal async Task<IEnumerable<string>> GetStoresAsync(string? search = null)
    {
        return await receiptRepository.GetStoresAsync(search);
    }

    internal async Task<IEnumerable<string>> GetItemsAsync(string? search = null)
    {
        return await receiptRepository.GetItemsAsync(search);
    }

    internal async Task<IEnumerable<string>> GetCategoriesAsync(string? search = null)
    {
        return await receiptRepository.GetCategoriesAsync(search);
    }

    internal async Task<Result<Guid, string>> CreateReceiptAsync(string store, DateOnly purchaseDate)
    {
        ValidationResult validationResult = Validate(store, purchaseDate);
        if (!validationResult.IsValid)
        {
            return Result<Guid, string>.Failure(string.Join(Environment.NewLine, validationResult.Errors));
        }

        ReceiptDetails receipt = new(Guid.CreateVersion7(), store, purchaseDate, 0);
        await receiptRepository.AddAsync(receipt);
        return Result.Success<Guid, string>(receipt.Id);
    }

    private static ValidationResult Validate(string store, DateOnly purchaseDate)
    {
        List<string> errors = [];
        if (string.IsNullOrWhiteSpace(store))
        {
            errors.Add("Invalid store");
        }

        if (purchaseDate > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("Invalid purchase date");
        }

        return new ValidationResult(errors);
    }

    private readonly record struct ValidationResult(IEnumerable<string> Errors)
    {
        public bool IsValid => !Errors.Any();
    }

    public async Task<Result<ReceiptWithPurchases, string>> GetReceiptAsync(Guid id)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(id);
        if (receipt == null)
        {
            return Result<ReceiptWithPurchases, string>.Failure("Receipt not found");
        }

        return Result<ReceiptWithPurchases, string>.Success(receipt);
    }

    public async Task<Result<Guid, string>> DuplicateReceipt(Guid id)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(id);
        if (receipt == null)
        {
            return Result<Guid, string>.Failure("Receipt not found");
        }

        Guid newId = Guid.CreateVersion7();
        ReceiptDetails duplicate = new(newId, receipt.Store, DateOnly.FromDateTime(DateTime.Today), 0);
        await receiptRepository.AddAsync(duplicate);
        foreach (PurchaseDetails purchase in receipt.Purchases)
        {
            PurchaseDetails newPurchase = new(
                Guid.CreateVersion7(),
                purchase.ItemName,
                purchase.Category,
                purchase.Quantity,
                purchase.UnitPrice,
                purchase.Discount,
                purchase.Description);

            await receiptRepository.AddPurchaseAsync(newId, newPurchase);
        }

        return Result<Guid, string>.Success(newId);
    }

    public async Task<Result<Unit, string>> DeleteReceiptAsync(Guid receiptId)
    {
        await receiptRepository.DeleteReceiptAsync(receiptId);
        return Result<Unit, string>.Success(Unit.Instance);
    }

    public async Task<Result<Unit, string>> AddPurchase(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(receiptId);
        if (receipt == null)
        {
            return Result<Unit, string>.Failure("Receipt not found");
        }

        await receiptRepository.AddPurchaseAsync(receiptId, purchase);
        return Result<Unit, string>.Success(Unit.Instance);
    }

    public async Task<Result<Unit, string>> UpdatePurchase(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(receiptId);
        if (receipt == null)
        {
            return Result<Unit, string>.Failure("Receipt not found");
        }

        await receiptRepository.UpdatePurchaseAsync(receiptId, purchase);
        return Result<Unit, string>.Success(Unit.Instance);
    }

    public async Task<Result<Unit, string>> DeletePurchaseAsync(Guid receiptId, Guid purchaseId)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(receiptId);
        if (receipt == null)
        {
            return Result<Unit, string>.Failure("Receipt not found");
        }

        await receiptRepository.DeletePurchaseAsync(receiptId, purchaseId);
        return Result<Unit, string>.Success(Unit.Instance);
    }
}