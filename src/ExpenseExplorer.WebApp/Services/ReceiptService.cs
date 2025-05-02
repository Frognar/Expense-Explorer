using System.Linq.Expressions;
using ExpenseExplorer.WebApp.Models;

namespace ExpenseExplorer.WebApp.Services;

#pragma warning disable CA1812
#pragma warning disable S2325
#pragma warning disable CA1822
internal sealed class ReceiptService
{
    private static readonly List<ReceiptDetails> Receipts =
        Enumerable.Range(1, 10000)
            .Select(i => new ReceiptDetails(
                Guid.CreateVersion7(),
                $"Store {i}",
                DateOnly.FromDateTime(DateTime.Today).AddDays(-i),
                i * 10m))
            .ToList();

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
        var x = Receipts
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

        await Task.CompletedTask;
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

    internal async Task<ICreateResult> CreateReceiptAsync(string store, DateOnly purchaseDate)
    {
        if (string.IsNullOrWhiteSpace(store))
        {
            return new ErrorCreateResult("Invalid store");
        }

        if (purchaseDate > DateOnly.FromDateTime(DateTime.Today))
        {
            return new ErrorCreateResult("Invalid purchase date");
        }

        ReceiptDetails receipt = new(Guid.CreateVersion7(), store, purchaseDate, 0);
        Receipts.Add(receipt);
        await Task.CompletedTask;
        return new SuccessCreateResult(receipt.Id);
    }

    public async Task<ReceiptWithPurchases> GetReceiptAsync(Guid id)
    {
        ReceiptDetails? receipt = Receipts.FirstOrDefault(r => r.Id == id);
        if (receipt == null)
        {
            throw new InvalidOperationException("Receipt not found");
        }

        await Task.CompletedTask;
        return new ReceiptWithPurchases(receipt.Id, receipt.Store, receipt.PurchaseDate,
            [
                new PurchaseDetails(
                    Guid.CreateVersion7(),
                    "Item 1",
                    "Category 1",
                    1,
                    2,
                    null,
                    null)
            ]);
    }
}

internal interface ICreateResult;
internal sealed record SuccessCreateResult(Guid Id): ICreateResult;
internal sealed record ErrorCreateResult(string Error) : ICreateResult;