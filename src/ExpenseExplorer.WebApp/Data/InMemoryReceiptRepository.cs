using ExpenseExplorer.WebApp.Models;

namespace ExpenseExplorer.WebApp.Data;

internal sealed class InMemoryReceiptRepository : IReceiptRepository
{
    private static readonly List<ReceiptWithPurchases> Receipts =
        Enumerable.Range(1, 10000)
            .Select(i => new ReceiptWithPurchases(
                Guid.CreateVersion7(),
                $"Store {i}",
                DateOnly.FromDateTime(DateTime.Today).AddDays(-i),
                []))
            .ToList();

    public async Task<ReceiptDetailsPage> GetReceiptsAsync(
        int pageSize,
        int skip,
        string orderBy,
        SortDirection sortDirection,
        IEnumerable<string> stores,
        DateOnly? purchaseDateFrom,
        DateOnly? purchaseDateTo,
        decimal? totalCostMin,
        decimal? totalCostMax)
    {
        IEnumerable<ReceiptDetails> result = Receipts
            .Select(r => new ReceiptDetails(r.Id, r.Store, r.PurchaseDate, r.Purchases.Sum(p => p.TotalPrice)))
            .Where(r =>
                (!stores.Any() || stores.Contains(r.Store))
                && (purchaseDateFrom == null || r.PurchaseDate >= purchaseDateFrom)
                && (purchaseDateTo == null || r.PurchaseDate <= purchaseDateTo)
                && (totalCostMin == null || r.TotalCost >= totalCostMin)
                && (totalCostMax == null || r.TotalCost <= totalCostMax))
            .ToList();

        int count = result.Count();
        decimal totalCost = result.Sum(r => r.TotalCost);
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            result = sortDirection == SortDirection.Descending
                ? result.OrderByDescending(GetOrderBy(orderBy))
                : result.OrderBy(GetOrderBy(orderBy));
        }

        List<ReceiptDetails> data = result
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        await Task.CompletedTask;
        return new ReceiptDetailsPage(data, count, totalCost);
    }

    private static Func<ReceiptDetails, object> GetOrderBy(string orderBy)
    {
        return orderBy switch
            {
                nameof(ReceiptDetails.Store) => r => r.Store,
                nameof(ReceiptDetails.PurchaseDate) => r => r.PurchaseDate,
                nameof(ReceiptDetails.TotalCost) => r => r.TotalCost,
                _ => r => r.Id
            };
    }

    public async Task<IEnumerable<string>> GetStoresAsync(string? search = null)
    {
        IEnumerable<string> stores = Receipts.Select(r => r.Store);
        if (string.IsNullOrWhiteSpace(search))
        {
            return stores;
        }

        await Task.CompletedTask;
        return stores.Where(s => search
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(f => s.Contains(f, StringComparison.InvariantCultureIgnoreCase)));
    }

    public async Task<IEnumerable<string>> GetItemsAsync(string? search = null)
    {
        IEnumerable<string> items = Enumerable.Range(1, 100).Select(i => $"Item {i}");
        if (string.IsNullOrWhiteSpace(search))
        {
            return items;
        }

        await Task.CompletedTask;
        return items.Where(s => search
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(f => s.Contains(f, StringComparison.InvariantCultureIgnoreCase)));
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(string? search = null)
    {
        IEnumerable<string> categories = Enumerable.Range(1, 100).Select(i => $"Category {i}");
        if (string.IsNullOrWhiteSpace(search))
        {
            return categories;
        }

        await Task.CompletedTask;
        return categories.Where(s => search
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(f => s.Contains(f, StringComparison.InvariantCultureIgnoreCase)));
    }

    public async Task AddAsync(ReceiptDetails receipt)
    {
        Receipts.Add(new ReceiptWithPurchases(receipt.Id, receipt.Store, receipt.PurchaseDate, []));
        await Task.CompletedTask;
    }

    public async Task<ReceiptWithPurchases?> GetReceiptAsync(Guid id)
    {
        ReceiptWithPurchases? receipt = Receipts.FirstOrDefault(r => r.Id == id);
        if (receipt == null)
        {
            return null;
        }

        await Task.CompletedTask;
        return receipt;
    }

    public async Task DeleteReceiptAsync(Guid id)
    {
        Receipts.RemoveAll(r => r.Id == id);
        await Task.CompletedTask;
    }

    public Task AddPurchaseAsync(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases receipt = Receipts.FirstOrDefault(r => r.Id == receiptId)!;
        int index = Receipts.IndexOf(receipt);
        Receipts[index] = receipt with { Purchases = receipt.Purchases.Append(purchase).ToList() };
        return Task.CompletedTask;
    }

    public Task UpdatePurchaseAsync(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases receipt = Receipts.FirstOrDefault(r => r.Id == receiptId)!;
        int index = Receipts.IndexOf(receipt);
        Receipts[index] = receipt with { Purchases = UpdatePurchase(receipt.Purchases, purchase) };
        return Task.CompletedTask;
    }

    private static IEnumerable<PurchaseDetails> UpdatePurchase(IEnumerable<PurchaseDetails> purchases, PurchaseDetails purchase)
    {
        foreach (PurchaseDetails purchaseDetails in purchases)
        {
            if (purchaseDetails.Id == purchase.Id)
            {
                yield return purchase;
            }
            else
            {
                yield return purchaseDetails;
            }
        }
    }
}