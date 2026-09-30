using System.Linq.Expressions;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Receipts;

internal sealed class ReceiptQueries(ExpenseExplorerDbContext db) : IReceiptQueries
{
    public async Task<ReceiptResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        ReceiptRow? row = await db.Receipts
            .AsNoTracking()
            .Include(r => r.Items)
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        return row is null ? null : ReceiptResponses.From(ReceiptMapping.ToDomain(row));
    }

    public async Task<ReceiptListResponse> ListAsync(ReceiptListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<SummaryRow> filtered = Filter(Summaries(db.Receipts.AsNoTracking()), query);

        int totalCount = await filtered.CountAsync(cancellationToken);
        decimal totalCost = await filtered.SumAsync(r => r.Total, cancellationToken);
        List<ReceiptSummaryResponse> page = await Sort(filtered, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new ReceiptSummaryResponse(r.Id, r.Store, r.PurchaseDate, r.Total))
            .ToListAsync(cancellationToken);

        return new ReceiptListResponse(
            new PageResponse<ReceiptSummaryResponse>(page, query.Page, query.PageSize, totalCount),
            totalCost);
    }

    /// <summary>
    /// Totals computed in SQL the same way as <see cref="Domain.Receipts.LinePrice"/>:
    /// unit price × quantity rounded to grosze, minus the discount.
    /// </summary>
    private static IQueryable<SummaryRow> Summaries(IQueryable<ReceiptRow> receipts) =>
        receipts.Select(r => new SummaryRow
        {
            Id = r.Id,
            Store = r.Store,
            PurchaseDate = r.PurchaseDate,
            // PostgreSQL round() on numeric rounds half away from zero, like the domain.
            Total = r.Items.Sum(i => Math.Round(i.UnitPrice * i.Quantity, 2) - i.Discount),
        });

    private static IQueryable<SummaryRow> Filter(IQueryable<SummaryRow> receipts, ReceiptListQuery query)
    {
        if (query.Stores.Count > 0)
        {
            string[] stores = [.. query.Stores];
            receipts = receipts.Where(r => stores.Contains(r.Store));
        }

        if (query.From is { } from)
        {
            receipts = receipts.Where(r => r.PurchaseDate >= from);
        }

        if (query.To is { } to)
        {
            receipts = receipts.Where(r => r.PurchaseDate <= to);
        }

        if (query.TotalMin is { } min)
        {
            receipts = receipts.Where(r => r.Total >= min);
        }

        if (query.TotalMax is { } max)
        {
            receipts = receipts.Where(r => r.Total <= max);
        }

        return receipts;
    }

    private static IOrderedQueryable<SummaryRow> Sort(IQueryable<SummaryRow> receipts, ReceiptListQuery query)
    {
        Expression<Func<SummaryRow, object>> key = query.SortBy switch
        {
            ReceiptSortField.Store => r => r.Store,
            ReceiptSortField.Total => r => r.Total,
            _ => r => r.PurchaseDate,
        };

        IOrderedQueryable<SummaryRow> sorted = query.Direction == SortDirection.Descending
            ? receipts.OrderByDescending(key)
            : receipts.OrderBy(key);

        // Stable paging when many receipts share the sort key.
        return sorted.ThenBy(r => r.Id);
    }

    /// <summary>Query-side shape; EF can filter and sort on its members before the final projection.</summary>
    private sealed class SummaryRow
    {
        public Guid Id { get; init; }

        public string Store { get; init; } = "";

        public DateOnly PurchaseDate { get; init; }

        public decimal Total { get; init; }
    }
}
