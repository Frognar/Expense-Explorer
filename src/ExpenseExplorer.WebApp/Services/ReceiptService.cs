using DotMaybe;
using DotResult;
using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;
using ExpenseExplorer.Application.Features.Receipts.AddItem;
using ExpenseExplorer.Application.Features.Receipts.CreateHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteItem;
using ExpenseExplorer.Application.Features.Receipts.Duplicate;
using ExpenseExplorer.Application.Features.Receipts.GetReceipt;
using ExpenseExplorer.Application.Features.Receipts.GetReceipts;
using ExpenseExplorer.Application.Features.Receipts.UpdateHeader;
using ExpenseExplorer.Application.Features.Receipts.UpdateItem;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.Queries;
using ExpenseExplorer.WebApp.Models;
using GetReceiptByIdQuery = ExpenseExplorer.Application.Features.Receipts.GetReceipt.GetReceiptByIdQuery;

namespace ExpenseExplorer.WebApp.Services;

internal sealed class ReceiptService(
    ICommandHandler<CreateReceiptHeaderRequest, CreateReceiptHeaderResponse> createReceiptHeaderCommandHandler,
#pragma warning disable CS9113 // Parameter is unread.
    ICommandHandler<UpdateReceiptHeaderRequest, UpdateReceiptHeaderResponse> updateReceiptHeaderCommandHandler,
#pragma warning restore CS9113 // Parameter is unread.
    ICommandHandler<DeleteReceiptHeaderRequest, DeleteReceiptHeaderResponse> deleteReceiptHeaderCommandHandler,
    ICommandHandler<DuplicateReceiptRequest, DuplicateReceiptResponse> duplicateReceiptCommandHandler,
    ICommandHandler<AddReceiptItemRequest, AddReceiptItemResponse> addReceiptItemCommandHandler,
    ICommandHandler<UpdateReceiptItemRequest, UpdateReceiptItemResponse> updateReceiptItemCommandHandler,
    ICommandHandler<DeleteReceiptItemRequest, DeleteReceiptItemResponse> deleteReceiptItemCommandHandler,
    IQueryHandler<GetReceiptByIdQuery, GetReceiptByIdResponse> getReceiptByIdQueryHandler,
    IQueryHandler<GetReceiptSummariesQuery, GetReceiptSummariesResponse> getReceiptSummariesQueryHandler,
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
        GetReceiptSummariesQuery query = GetReceiptSummariesQuery.Default
            .Where(ReceiptFilter.StoresIn(stores))
            .Where(ReceiptFilter.PurchaseDateBetween(purchaseDateFrom, purchaseDateTo))
            .Where(ReceiptFilter.TotalBetween(totalCostMin, totalCostMax))
            .GetPage(pageSize, skip);

        ReceiptOrder order = orderBy?
                .Replace("desc", "", StringComparison.CurrentCultureIgnoreCase)
                .Replace("asc", "", StringComparison.CurrentCultureIgnoreCase)
                .Trim() switch
        {
            nameof(Models.ReceiptDetails.Store) => ReceiptOrder.Store,
            nameof(Models.ReceiptDetails.PurchaseDate) => ReceiptOrder.PurchaseDate,
            nameof(Models.ReceiptDetails.TotalCost) => ReceiptOrder.Total,
            _ => ReceiptOrder.Id
        };

        query = orderBy?.Contains("desc", StringComparison.InvariantCultureIgnoreCase) == true
            ? query.OrderByDescending(order)
            : query.OrderBy(order);

        Result<GetReceiptSummariesResponse> result =
            await getReceiptSummariesQueryHandler.HandleAsync(query, CancellationToken.None);

        return result.Match(
            failure: _ => new ReceiptDetailsPage([], 0, 0),
            success: page => new ReceiptDetailsPage(
                page.Receipts.Items.Select(r => new Models.ReceiptDetails(
                    r.Id,
                    r.Store,
                    r.PurchaseDate,
                    r.Total)),
                    (int)page.Receipts.TotalCount,
                    page.Total));
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

    internal async Task<Result<Guid>> CreateReceiptAsync(string store, DateOnly purchaseDate)
    {
        CreateReceiptHeaderRequest request = new(store, purchaseDate, DateOnly.FromDateTime(DateTime.Today));
        return await createReceiptHeaderCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(response => response.ReceiptId);
    }

    public async Task<Result<Maybe<ReceiptWithPurchases>>> GetReceiptAsync(Guid id)
    {
        GetReceiptByIdQuery query = new(id);
        return await getReceiptByIdQueryHandler.HandleAsync(query, CancellationToken.None)
            .MapAsync(response =>
                response.Receipt.Map(r =>
                    new ReceiptWithPurchases(
                        r.Id,
                        r.Store,
                        r.PurchaseDate,
                        r.Items.Select(i =>
                            new PurchaseDetails(
                                i.Id,
                                i.Item,
                                i.Category,
                                i.Quantity,
                                i.UnitPrice,
                                i.Discount.Match<decimal?>(none: () => null, some: m => m),
                                i.Description.Match<string?>(none: () => null, some: m => m))))));
    }

    public async Task<Result<Guid>> DuplicateReceipt(Guid id)
    {
        DuplicateReceiptRequest request = new(id, DateOnly.FromDateTime(DateTime.Today));
        return await duplicateReceiptCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(response => response.ReceiptId);
    }

    public async Task<Result<Unit>> DeleteReceiptAsync(Guid receiptId)
    {
        DeleteReceiptHeaderRequest request = new(receiptId);
        return await deleteReceiptHeaderCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(_ => Unit.Instance);
    }

    public async Task<Result<Guid>> AddPurchase(Guid receiptId, PurchaseDetails purchase)
    {
        AddReceiptItemRequest request = new(
            receiptId,
            purchase.ItemName,
            purchase.Category,
            purchase.Quantity,
            purchase.UnitPrice,
            purchase.Discount,
            purchase.Description);

        return await addReceiptItemCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(response => response.ReceiptItemId);
    }

    public async Task<Result<Unit>> UpdatePurchase(Guid receiptId, PurchaseDetails purchase)
    {
        UpdateReceiptItemRequest request = new(
            purchase.Id,
            receiptId,
            purchase.ItemName,
            purchase.Category,
            purchase.Quantity,
            purchase.UnitPrice,
            purchase.Discount,
            purchase.Description);

        return await updateReceiptItemCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(_ => Unit.Instance);
    }

    public async Task<Result<Unit>> DeletePurchaseAsync(Guid receiptId, Guid purchaseId)
    {
        DeleteReceiptItemRequest request = new(receiptId, purchaseId);
        return await deleteReceiptItemCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(_ => Unit.Instance);
    }
}