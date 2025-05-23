using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.ValueObjects;
using ExpenseExplorer.WebApp.Data;
using ExpenseExplorer.WebApp.Models;
using IReceiptRepository = ExpenseExplorer.WebApp.Data.IReceiptRepository;

namespace ExpenseExplorer.WebApp.Services;

internal sealed class ReceiptService(
    IReceiptRepository receiptRepository,
    ExpenseExplorer.Application.Receipts.Data.IReceiptRepository applicationReceiptRepository)
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
        CreateReceiptCommand command = new(store, purchaseDate, DateOnly.FromDateTime(DateTime.Today));
        CreateReceiptHandler handler = new(applicationReceiptRepository);
        Result<ReceiptId, IEnumerable<string>> result = await handler.HandleAsync(command, CancellationToken.None);
        return result
            .MapError(errors => string.Join(Environment.NewLine, errors))
            .Map(receiptId => receiptId.Value);
    }

    public async Task<Result<ReceiptWithPurchases, string>> GetReceiptAsync(Guid id)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(id);
        if (receipt == null)
        {
            return Result.Failure<ReceiptWithPurchases, string>("Receipt not found");
        }

        return Result.Success<ReceiptWithPurchases, string>(receipt);
    }

    public async Task<Result<Guid, string>> DuplicateReceipt(Guid id)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(id);
        if (receipt == null)
        {
            return Result.Failure<Guid, string>("Receipt not found");
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

        return Result.Success<Guid, string>(newId);
    }

    public async Task<Result<Unit, string>> DeleteReceiptAsync(Guid receiptId)
    {
        await receiptRepository.DeleteReceiptAsync(receiptId);
        return Result.Success<Unit, string>(Unit.Instance);
    }

    public async Task<Result<Unit, string>> AddPurchase(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(receiptId);
        if (receipt == null)
        {
            return Result.Failure<Unit, string>("Receipt not found");
        }

        await receiptRepository.AddPurchaseAsync(receiptId, purchase);
        return Result.Success<Unit, string>(Unit.Instance);
    }

    public async Task<Result<Unit, string>> UpdatePurchase(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(receiptId);
        if (receipt == null)
        {
            return Result.Failure<Unit, string>("Receipt not found");
        }

        await receiptRepository.UpdatePurchaseAsync(receiptId, purchase);
        return Result.Success<Unit, string>(Unit.Instance);
    }

    public async Task<Result<Unit, string>> DeletePurchaseAsync(Guid receiptId, Guid purchaseId)
    {
        ReceiptWithPurchases? receipt = await receiptRepository.GetReceiptAsync(receiptId);
        if (receipt == null)
        {
            return Result.Failure<Unit, string>("Receipt not found");
        }

        await receiptRepository.DeletePurchaseAsync(receiptId, purchaseId);
        return Result.Success<Unit, string>(Unit.Instance);
    }
}