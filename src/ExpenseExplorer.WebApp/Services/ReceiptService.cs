using DotMaybe;
using DotResult;
using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Features.Categories.GetCategories;
using ExpenseExplorer.Application.Features.Items.GetItems;
using ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;
using ExpenseExplorer.Application.Features.Receipts.AddItem;
using ExpenseExplorer.Application.Features.Receipts.CreateHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteItem;
using ExpenseExplorer.Application.Features.Receipts.Duplicate;
using ExpenseExplorer.Application.Features.Receipts.GetReceipt;
using ExpenseExplorer.Application.Features.Receipts.GetReceipts;
using ExpenseExplorer.Application.Features.Receipts.UpdateHeader;
using ExpenseExplorer.Application.Features.Receipts.UpdateItem;
using ExpenseExplorer.Application.Features.Reports.GetCategoryReport;
using ExpenseExplorer.Application.Features.Stores.GetStores;
using ExpenseExplorer.WebApp.Models;
using GetReceiptByIdQuery = ExpenseExplorer.Application.Features.Receipts.GetReceipt.GetReceiptByIdQuery;

namespace ExpenseExplorer.WebApp.Services;

internal sealed class ReceiptService(
    ICommandHandler<CreateReceiptHeaderRequest, CreateReceiptHeaderResponse> createReceiptHeaderCommandHandler,
    ICommandHandler<UpdateReceiptHeaderRequest, UpdateReceiptHeaderResponse> updateReceiptHeaderCommandHandler,
    ICommandHandler<DeleteReceiptHeaderRequest, DeleteReceiptHeaderResponse> deleteReceiptHeaderCommandHandler,
    ICommandHandler<DuplicateReceiptRequest, DuplicateReceiptResponse> duplicateReceiptCommandHandler,
    ICommandHandler<AddReceiptItemRequest, AddReceiptItemResponse> addReceiptItemCommandHandler,
    ICommandHandler<UpdateReceiptItemRequest, UpdateReceiptItemResponse> updateReceiptItemCommandHandler,
    ICommandHandler<DeleteReceiptItemRequest, DeleteReceiptItemResponse> deleteReceiptItemCommandHandler,
    IQueryHandler<GetReceiptByIdQuery, GetReceiptByIdResponse> getReceiptByIdQueryHandler,
    IQueryHandler<GetReceiptSummariesQuery, GetReceiptSummariesResponse> getReceiptSummariesQueryHandler,
    IQueryHandler<GetStoresRequest, GetStoresResponse> getStoresQueryHandler,
    IQueryHandler<GetItemsRequest, GetItemsResponse> getItemsQueryHandler,
    IQueryHandler<GetCategoriesRequest, GetCategoriesResponse> getCategoriesQueryHandler,
    IQueryHandler<GetReceiptItemsQuery, GetReceiptItemsResponse> getReceiptItemsQueryHandler,
    IQueryHandler<GetCategoryReportQuery, GetCategoryReportResponse> getCategoryReportQueryHandler)
{
    public async Task<IEnumerable<CategoryExpense>> GetCategoryReportAsync(DateOnly? from, DateOnly? to)
    {
        GetCategoryReportQuery query = new(from ?? DateOnly.MinValue, to ?? DateOnly.MaxValue);
        Result<GetCategoryReportResponse> result = await getCategoryReportQueryHandler.HandleAsync(query, CancellationToken.None);
        return result.Match(
            failure: _ => [],
            success: response => response.Categories);
    }

    public async Task<ReceiptItemDetailsPage> GetReceiptItemsAsync(
        int pageSize,
        int skip,
        string? orderBy,
        IEnumerable<string> stores,
        IEnumerable<string> items,
        IEnumerable<string> categories,
        DateOnly? purchaseDateFrom,
        DateOnly? purchaseDateTo,
        decimal? unitPriceMin,
        decimal? unitPriceMax,
        decimal? quantityMin,
        decimal? quantityMax,
        decimal? discountMin,
        decimal? discountMax,
        decimal? totalCostMin,
        decimal? totalCostMax,
        string? description)
    {
        GetReceiptItemsQuery query = GetReceiptItemsQuery.Default
            .Where(ReceiptItemFilter.StoresIn(stores))
            .Where(ReceiptItemFilter.ItemsIn(items))
            .Where(ReceiptItemFilter.CategoriesIn(categories))
            .Where(ReceiptItemFilter.PurchaseDateBetween(purchaseDateFrom, purchaseDateTo))
            .Where(ReceiptItemFilter.UnitPriceBetween(unitPriceMin, unitPriceMax))
            .Where(ReceiptItemFilter.QuantityBetween(quantityMin, quantityMax))
            .Where(ReceiptItemFilter.DiscountBetween(discountMin, discountMax))
            .Where(ReceiptItemFilter.TotalBetween(totalCostMin, totalCostMax))
            .Where(ReceiptItemFilter.DescriptionContains(description))
            .GetPage(pageSize, skip);

        ReceiptItemOrder order = orderBy?
                .Replace("desc", "", StringComparison.CurrentCulture)
                .Replace("asc", "", StringComparison.CurrentCulture)
                .Trim() switch
        {
            nameof(Models.ReceiptItemDetails.Store) => ReceiptItemOrder.Store,
            nameof(Models.ReceiptItemDetails.PurchaseDate) => ReceiptItemOrder.PurchaseDate,
            nameof(Models.ReceiptItemDetails.Item) => ReceiptItemOrder.Item,
            nameof(Models.ReceiptItemDetails.Category) => ReceiptItemOrder.Category,
            nameof(Models.ReceiptItemDetails.UnitPrice) => ReceiptItemOrder.UnitPrice,
            nameof(Models.ReceiptItemDetails.Quantity) => ReceiptItemOrder.Quantity,
            nameof(Models.ReceiptItemDetails.Discount) => ReceiptItemOrder.Discount,
            nameof(Models.ReceiptItemDetails.Total) => ReceiptItemOrder.Total,
            nameof(Models.ReceiptItemDetails.Description) => ReceiptItemOrder.Description,
            _ => ReceiptItemOrder.Id
        };

        query = orderBy?.EndsWith("desc", StringComparison.CurrentCulture) == true
            ? query.OrderByDescending(order)
            : query.OrderBy(order);

        Result<GetReceiptItemsResponse> result =
            await getReceiptItemsQueryHandler.HandleAsync(query, CancellationToken.None);

        return result.Match(
            failure: _ => new ReceiptItemDetailsPage([], 0, 0),
            success: page => new ReceiptItemDetailsPage(
                page.Items.Items.Select(r => new Models.ReceiptItemDetails(
                    r.Id,
                    r.ReceiptId,
                    r.Store,
                    r.PurchaseDate,
                    r.Item,
                    r.Category,
                    r.UnitPrice,
                    r.Quantity,
                    r.Discount.Match<decimal?>(() => null, d => d),
                    r.UnitPrice * r.Quantity - r.Discount.OrDefault(0m),
                    r.Description.Match<string?>(() => null, d => d))),
                    (int)page.Items.TotalCount,
                    page.Total));
    }

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
                .Replace("desc", "", StringComparison.CurrentCulture)
                .Replace("asc", "", StringComparison.CurrentCulture)
                .Trim() switch
        {
            nameof(Models.ReceiptDetails.Store) => ReceiptOrder.Store,
            nameof(Models.ReceiptDetails.PurchaseDate) => ReceiptOrder.PurchaseDate,
            nameof(Models.ReceiptDetails.TotalCost) => ReceiptOrder.Total,
            _ => ReceiptOrder.Id
        };

        query = orderBy?.Contains("desc", StringComparison.CurrentCulture) == true
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

    public async Task<IEnumerable<string>> GetStoresAsync(string? search = null)
    {
        GetStoresRequest request = new(search is not null ? Some.With(search) : None.OfType<string>());
        Result<GetStoresResponse> response = await getStoresQueryHandler.HandleAsync(request, CancellationToken.None);
        return response
            .Match(_ => [], s => s.Stores);
    }

    public async Task<IEnumerable<string>> GetItemsAsync(string? search = null)
    {
        GetItemsRequest request = new(search is not null ? Some.With(search) : None.OfType<string>());
        Result<GetItemsResponse> response = await getItemsQueryHandler.HandleAsync(request, CancellationToken.None);
        return response
            .Match(_ => [], s => s.Items);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(string? search = null)
    {
        GetCategoriesRequest request = new(search is not null ? Some.With(search) : None.OfType<string>());
        Result<GetCategoriesResponse> response = await getCategoriesQueryHandler
            .HandleAsync(request, CancellationToken.None);

        return response
            .Match(_ => [], s => s.Categories);
    }

    public async Task<Result<Guid>> CreateReceiptAsync(string store, DateOnly purchaseDate)
    {
        CreateReceiptHeaderRequest request = new(store, purchaseDate, DateOnly.FromDateTime(DateTime.Today));
        return await createReceiptHeaderCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(response => response.ReceiptId);
    }

    public async Task<Result<Unit>> UpdateReceiptAsync(Guid receiptId, string store, DateOnly purchaseDate)
    {
        UpdateReceiptHeaderRequest request = new(receiptId, store, purchaseDate, DateOnly.FromDateTime(DateTime.Today));
        return await updateReceiptHeaderCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(_ => Unit.Instance);
    }

    public async Task<Result<Unit>> DeleteReceiptAsync(Guid receiptId)
    {
        DeleteReceiptHeaderRequest request = new(receiptId);
        return await deleteReceiptHeaderCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(_ => Unit.Instance);
    }

    public async Task<Result<Guid>> DuplicateReceipt(Guid id)
    {
        DuplicateReceiptRequest request = new(id, DateOnly.FromDateTime(DateTime.Today));
        return await duplicateReceiptCommandHandler.HandleAsync(request, CancellationToken.None)
            .MapAsync(response => response.ReceiptId);
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