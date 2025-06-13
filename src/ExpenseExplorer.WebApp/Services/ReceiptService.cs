using DotMaybe;
using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.Queries;
using ExpenseExplorer.Application.Receipts.ValueObjects;
using ExpenseExplorer.WebApp.Models;
using IReceiptRepository = ExpenseExplorer.WebApp.Data.IReceiptRepository;
using ReceiptDetails = ExpenseExplorer.WebApp.Models.ReceiptDetails;

namespace ExpenseExplorer.WebApp.Services;

internal sealed class ReceiptService(
    IReceiptRepository receiptRepository,
    ExpenseExplorer.Application.Receipts.Data.IReceiptRepository applicationReceiptRepository,
    IReceiptCommandRepository commandRepository,
    IStoreRepository storeRepository,
    IItemRepository itemRepository,
    ICategoryRepository categoryRepository)
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
        GetReceiptsQuery query = GetReceiptsQuery.Default
            .Where(ReceiptFilter.StoresIn(stores))
            .Where(ReceiptFilter.PurchaseDateBetween(purchaseDateFrom, purchaseDateTo))
            .Where(ReceiptFilter.TotalCostBetween(totalCostMin, totalCostMax))
            .GetPage(pageSize, skip);

        ReceiptOrder order = orderBy?.Replace("desc", "", StringComparison.CurrentCultureIgnoreCase).Trim() switch
        {
            nameof(ReceiptDetails.Store) => ReceiptOrder.Store,
            nameof(ReceiptDetails.PurchaseDate) => ReceiptOrder.PurchaseDate,
            nameof(ReceiptDetails.TotalCost) => ReceiptOrder.TotalCost,
            _ => ReceiptOrder.Id
        };

        query = orderBy?.Contains("desc", StringComparison.InvariantCultureIgnoreCase) == true
            ? query.OrderByDescending(order)
            : query.OrderBy(order);

        GetReceiptsHandler handler = new(applicationReceiptRepository);
        (PageOf<ReceiptSummary> pageOfReceipts, Money total) = await handler.HandleAsync(query, CancellationToken.None);

        return new ReceiptDetailsPage(
            pageOfReceipts.Items.Select(r => new ReceiptDetails(r.Id.Value, r.Store.Name, r.PurchaseDate.Date, r.Total.Value)),
            (int)pageOfReceipts.TotalCount,
            total.Value);
    }

    internal async Task<IEnumerable<string>> GetStoresAsync(string? search = null)
    {
        GetStoresQuery query = new(search is not null ? Some.With(search) : None.OfType<string>());
        GetStoresHandler handler = new(storeRepository);
        IEnumerable<Store> stores = await handler.HandleAsync(query, CancellationToken.None);
        return stores.Select(s => s.Name);
    }

    internal async Task<IEnumerable<string>> GetItemsAsync(string? search = null)
    {
        GetItemsQuery query = new(search is not null ? Some.With(search) : None.OfType<string>());
        GetItemsHandler handler = new(itemRepository);
        IEnumerable<Item> items = await handler.HandleAsync(query, CancellationToken.None);
        return items.Select(i => i.Name);
    }

    internal async Task<IEnumerable<string>> GetCategoriesAsync(string? search = null)
    {
        GetCategoriesQuery query = new(search is not null ? Some.With(search) : None.OfType<string>());
        GetCategoriesHandler handler = new(categoryRepository);
        IEnumerable<Category> categories = await handler.HandleAsync(query, CancellationToken.None);
        return categories.Select(c => c.Name);
    }

    internal async Task<Result<Guid, string>> CreateReceiptAsync(string store, DateOnly purchaseDate)
    {
        CreateReceiptCommand command = new(store, purchaseDate, DateOnly.FromDateTime(DateTime.Today));
        CreateReceiptHandler handler = new(commandRepository);
        Result<ReceiptId, IEnumerable<string>> result = await handler.HandleAsync(command, CancellationToken.None);
        return result
            .MapError(errors => string.Join(Environment.NewLine, errors))
            .Map(receiptId => receiptId.Value);
    }

    public async Task<Result<ReceiptWithPurchases, string>> GetReceiptAsync(Guid id)
    {
        GetReceiptByIdQuery query = new(id);
        GetReceiptByIdHandler handler = new(applicationReceiptRepository);
        Result<Application.Receipts.DTO.ReceiptDetails, ValidationError> response = await handler.HandleAsync(query, CancellationToken.None);
        return response.MapError(e => e.Error)
            .Map(r =>
                new ReceiptWithPurchases(
                    r.Id.Value,
                    r.Store.Name,
                    r.PurchaseDate.Date,
                    r.Items.Select(i =>
                        new PurchaseDetails(
                            i.Id.Value,
                            i.Item.Name,
                            i.Category.Name,
                            i.Quantity.Value,
                            i.UnitPrice.Value,
                            i.Discount.Match<decimal?>(none: () => null, some: m => m.Value),
                            i.Description.Match<string?>(none: () => null, some: m => m.Value)))));
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
        DeleteReceiptCommand command = new(receiptId);
        DeleteReceiptHandler handler = new(commandRepository);
        Result<Unit, ValidationError> result = await handler.HandleAsync(command, CancellationToken.None);
        return result
            .MapError(errors => string.Join(Environment.NewLine, errors));
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