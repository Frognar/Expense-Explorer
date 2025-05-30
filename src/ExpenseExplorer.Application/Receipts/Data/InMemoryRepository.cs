using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

internal sealed class InMemoryRepository : IReceiptRepository
{
    private static readonly List<ReceiptDetails> Receipts = [];

    public Task<Unit> CreateReceipt(CreateReceiptRequest receipt, CancellationToken cancellationToken)
    {
        Receipts.Add(new ReceiptDetails(receipt.Id, receipt.Store, receipt.PurchaseDate, []));
        return Task.FromResult(Unit.Instance);
    }

    public Task<Unit> DeleteReceipt(ReceiptId id, CancellationToken cancellationToken)
    {
        Receipts.RemoveAll(r => r.Id == id);
        return Task.FromResult(Unit.Instance);
    }

    public async Task<PageOf<ReceiptSummary>> GetReceiptsAsync(
        int pageSize,
        int skip,
        ReceiptOrder order,
        ImmutableArray<ReceiptFilter> filters,
        CancellationToken cancellationToken)
    {
        IEnumerable<ReceiptSummary> receipts = GetReceipts(filters);
        int count = receipts.Count();
        receipts = order.Descending
            ? receipts.OrderByDescending(GetOrderBy(order.OrderBy))
            : receipts.OrderBy(GetOrderBy(order.OrderBy));

        List<ReceiptSummary> r = receipts.Skip(skip).Take(pageSize).ToList();
        await Task.CompletedTask;
        return Page.Of([..r], (uint)count);
    }

    private static IEnumerable<ReceiptSummary> GetReceipts(
        ImmutableArray<ReceiptFilter> filters)
    {
        return Receipts
            .Select(r => new ReceiptSummary(
                r.Id,
                r.Store,
                r.PurchaseDate,
                r.Items.Aggregate(
                    Money.Zero, (m, i) => m + (i.UnitPrice * i.Quantity - i.Discount.OrElse(() => Money.Zero)))))
            .Where(MatchesReceiptFilters(filters));
    }

    private static Func<ReceiptSummary, bool> MatchesReceiptFilters(ImmutableArray<ReceiptFilter> filters) =>
        r => filters.All(f => MatchesReceiptFilter(r, f));

    private static bool MatchesReceiptFilter(ReceiptSummary receipt, ReceiptFilter filter) =>
        (!filter.Stores.Any() || filter.Stores.Contains(receipt.Store.Name))
        && (filter.PurchaseDateFrom == null || receipt.PurchaseDate.Date >= filter.PurchaseDateFrom)
        && (filter.PurchaseDateTo == null || receipt.PurchaseDate.Date <= filter.PurchaseDateTo)
        && (filter.TotalCostMin == null || receipt.Total.Value >= filter.TotalCostMin)
        && (filter.TotalCostMax == null || receipt.Total.Value <= filter.TotalCostMax);

    private static Func<ReceiptSummary, object> GetOrderBy(string orderBy)
    {
        return orderBy switch
        {
            "Store" => r => r.Store.Name,
            "PurchaseDate" => r => r.PurchaseDate.Date,
            "TotalCost" => r => r.Total.Value,
            _ => r => r.Id.Value
        };
    }

    public async Task<Money> GetTotalCostAsync(
        ImmutableArray<ReceiptFilter> filters,
        CancellationToken cancellationToken)
    {
        IEnumerable<ReceiptSummary> receipts = GetReceipts(filters);
        await Task.CompletedTask;
        return receipts.Aggregate(Money.Zero, (m, r) => m + r.Total);
    }

    public async Task<Option<ReceiptDetails>> GetReceiptByIdAsync(ReceiptId id, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return Receipts.FirstOrDefault(r => r.Id == id) is { } receipt
            ? Option.Some(receipt)
            : Option.None<ReceiptDetails>();
    }

    public async Task<ImmutableArray<Store>> GetStoresAsync(Option<string> search, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        IEnumerable<Store> stores = Receipts.Select(r => r.Store).Distinct();
        return search.Match<ImmutableArray<Store>>(
            none: () => [..stores],
            some: searchTerm =>
            [
                ..stores
                    .Where(store => searchTerm
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .All(str => store.Name.Contains(str, StringComparison.InvariantCultureIgnoreCase)))
            ]);
    }

    public async Task<ImmutableArray<Item>> GetItemsAsync(Option<string> search, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        IEnumerable<Item> items = Receipts.SelectMany(r => r.Items.Select(i => i.Item)).Distinct();
        return search.Match<ImmutableArray<Item>>(
            none: () => [..items],
            some: searchTerm =>
            [
                ..items
                    .Where(item => searchTerm
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .All(str => item.Name.Contains(str, StringComparison.InvariantCultureIgnoreCase)))
            ]);
    }

    public async Task<ImmutableArray<Category>> GetCategoriesAsync(Option<string> search, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        IEnumerable<Category> categories = Receipts.SelectMany(r => r.Items.Select(i => i.Category)).Distinct();
        return search.Match<ImmutableArray<Category>>(
            none: () => [..categories],
            some: searchTerm =>
            [
                ..categories
                    .Where(category => searchTerm
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .All(str => category.Name.Contains(str, StringComparison.InvariantCultureIgnoreCase)))
            ]);
    }
}