using System.Linq.Expressions;
using ExpenseExplorer.WebApp.Models;

namespace ExpenseExplorer.WebApp.Services;

#pragma warning disable CA1812
internal sealed class ReceiptService
{
    private readonly IEnumerable<ReceiptDetails> _receipts =
        Enumerable.Range(1, 10000)
            .Select(i => new ReceiptDetails(
                Guid.CreateVersion7(),
                $"Store {i}",
                DateOnly.FromDateTime(DateTime.Today).AddDays(-i),
                i * 10m));

    public async Task<ReceiptDetailsResponse> GetReceiptsAsync(
        int pageSize,
        int skip,
        string? orderBy,
        IEnumerable<string> stores,
        DateOnly? purchaseDateFrom,
        DateOnly? purchaseDateTo,
        decimal? totalCostMin,
        decimal? totalCostMax)
    {
        await Task.Yield();
        var x = _receipts
            .AsQueryable()
            .Where(r =>
                (!stores.Any() || stores.Contains(r.Store))
                && (purchaseDateFrom == null || r.PurchaseDate >= purchaseDateFrom)
                && (purchaseDateTo == null || r.PurchaseDate <= purchaseDateTo)
                && (totalCostMin == null || r.TotalCost >= totalCostMin)
                && (totalCostMax == null || r.TotalCost <= totalCostMax));

        int count = x.Count();
        decimal totalCost = x.Sum(r => r.TotalCost);
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            x = orderBy.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase)
                ? x.OrderByDescending(GetOrderBy(orderBy))
                : x.OrderBy(GetOrderBy(orderBy));
        }

        List<ReceiptDetails> data = x
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return new ReceiptDetailsResponse(data, count, totalCost);
    }

    private static Expression<Func<ReceiptDetails, object>> GetOrderBy(string orderBy)
    {
        return orderBy
                .Replace(" asc", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" desc", "", StringComparison.InvariantCultureIgnoreCase) switch
        {
            nameof(ReceiptDetails.Store) => r => r.Store,
            nameof(ReceiptDetails.PurchaseDate) => r => r.PurchaseDate,
            nameof(ReceiptDetails.TotalCost) => r => r.TotalCost,
            _ => r => r.Id
        };
    }

    internal async Task<IEnumerable<string>> GetStores(string? search = null)
    {
        await Task.Yield();
        IEnumerable<string> stores = _receipts.Select(r => r.Store);
        if (string.IsNullOrWhiteSpace(search))
        {
            return stores;
        }

        return stores.Where(s => search
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(f => s.Contains(f, StringComparison.InvariantCultureIgnoreCase)));
    }
}