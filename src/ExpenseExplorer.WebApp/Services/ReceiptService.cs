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

    internal async Task<Result<Guid, string>> CreateReceiptAsync(string store, DateOnly purchaseDate)
    {
        if (string.IsNullOrWhiteSpace(store))
        {
            return Result.Failure<Guid, string>("Invalid store");
        }

        if (purchaseDate > DateOnly.FromDateTime(DateTime.Today))
        {
            return Result.Failure<Guid, string>("Invalid purchase date");
        }

        ReceiptDetails receipt = new(Guid.CreateVersion7(), store, purchaseDate, 0);
        await receiptRepository.AddAsync(receipt);
        return Result.Success<Guid, string>(receipt.Id);
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

#pragma warning disable CA1822
#pragma warning disable S2325
    public async Task<Result<Unit, string>> UpdatePurchase(PurchaseDetails purchase)
    {
        await Task.CompletedTask;
        return Result<Unit, string>.Failure("Hello there!");
    }

    public async Task<Result<Unit, string>> AddPurchase(Guid receiptId, PurchaseDetails purchase)
    {
        await Task.CompletedTask;
        return Result<Unit, string>.Failure("Hello there!");
    }
}