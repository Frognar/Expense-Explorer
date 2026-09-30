using System.Linq.Expressions;
using ExpenseExplorer.Application.ReceiptItems;
using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.ReceiptItems;

internal sealed class ReceiptItemQueries(ExpenseExplorerDbContext db) : IReceiptItemQueries
{
    public async Task<ReceiptItemListResponse> ListAsync(ReceiptItemListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<LineRow> filtered = Filter(Lines(), query);

        int totalCount = await filtered.CountAsync(cancellationToken);
        decimal totalCost = await filtered.SumAsync(line => line.Total, cancellationToken);
        List<ReceiptItemSummaryResponse> page = await Sort(filtered, query)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(line => new ReceiptItemSummaryResponse(
                line.Id,
                line.ReceiptId,
                line.Store,
                line.PurchaseDate,
                line.Item,
                line.Category,
                line.Quantity,
                line.UnitPrice,
                line.Amount,
                line.Discount,
                line.Total,
                line.Description))
            .ToListAsync(cancellationToken);

        return new ReceiptItemListResponse(
            new PageResponse<ReceiptItemSummaryResponse>(page, query.Page, query.PageSize, totalCount),
            totalCost);
    }

    /// <summary>Unit price and total computed in SQL the same way as <see cref="Domain.Receipts.LinePrice"/>.</summary>
    private IQueryable<LineRow> Lines() =>
        from item in db.ReceiptItems.AsNoTracking()
        join receipt in db.Receipts.AsNoTracking() on item.ReceiptId equals receipt.Id
        select new LineRow
        {
            Id = item.Id,
            ReceiptId = receipt.Id,
            Store = receipt.Store,
            PurchaseDate = receipt.PurchaseDate,
            Item = item.Item,
            Category = item.Category,
            Quantity = item.Quantity,
            UnitPrice = Math.Round(item.Amount / item.Quantity, Domain.Receipts.LinePrice.UnitPriceDecimalPlaces),
            Amount = item.Amount,
            Discount = item.Discount,
            Total = item.Amount - item.Discount,
            Description = item.Description,
        };

    private static IQueryable<LineRow> Filter(IQueryable<LineRow> lines, ReceiptItemListQuery query)
    {
        lines = OneOf(lines, query.Stores, line => line.Store);
        lines = OneOf(lines, query.Items, line => line.Item);
        lines = OneOf(lines, query.Categories, line => line.Category);
        lines = Within(lines, query.PurchaseDate, line => line.PurchaseDate);
        lines = Within(lines, query.Amounts.Quantity, line => line.Quantity);
        lines = Within(lines, query.Amounts.UnitPrice, line => line.UnitPrice);
        lines = Within(lines, query.Amounts.Amount, line => line.Amount);
        lines = Within(lines, query.Amounts.Discount, line => line.Discount);
        lines = Within(lines, query.Amounts.Total, line => line.Total);

        if (query.Description is { } description)
        {
            string pattern = $"%{EscapeLike(description)}%";
            lines = lines.Where(line => line.Description != null && EF.Functions.ILike(line.Description, pattern, @"\"));
        }

        return lines;
    }

    private static IQueryable<LineRow> OneOf(IQueryable<LineRow> lines, IReadOnlyList<string> values, Expression<Func<LineRow, string>> field)
    {
        if (values.Count == 0)
        {
            return lines;
        }

        string[] accepted = [.. values];
        Expression<Func<string, bool>> isAccepted = value => accepted.Contains(value);
        return lines.Where(Expression.Lambda<Func<LineRow, bool>>(
            Expression.Invoke(isAccepted, field.Body),
            field.Parameters));
    }

    private static IQueryable<LineRow> Within<T>(IQueryable<LineRow> lines, Range<T> range, Expression<Func<LineRow, T>> field)
        where T : struct
    {
        if (range.Min is { } min)
        {
            lines = lines.Where(Compare(field, Expression.GreaterThanOrEqual, min));
        }

        if (range.Max is { } max)
        {
            lines = lines.Where(Compare(field, Expression.LessThanOrEqual, max));
        }

        return lines;
    }

    /// <summary>Builds <c>line => field(line) [comparison] bound</c> as an expression EF can translate.</summary>
    private static Expression<Func<LineRow, bool>> Compare<T>(
        Expression<Func<LineRow, T>> field,
        Func<Expression, Expression, BinaryExpression> comparison,
        T bound) =>
        Expression.Lambda<Func<LineRow, bool>>(
            comparison(field.Body, Expression.Constant(bound, typeof(T))),
            field.Parameters);

    private static IOrderedQueryable<LineRow> Sort(IQueryable<LineRow> lines, ReceiptItemListQuery query)
    {
        Expression<Func<LineRow, object?>> key = query.SortBy switch
        {
            ReceiptItemSortField.Store => line => line.Store,
            ReceiptItemSortField.Item => line => line.Item,
            ReceiptItemSortField.Category => line => line.Category,
            ReceiptItemSortField.Quantity => line => line.Quantity,
            ReceiptItemSortField.UnitPrice => line => line.UnitPrice,
            ReceiptItemSortField.Amount => line => line.Amount,
            ReceiptItemSortField.Discount => line => line.Discount,
            ReceiptItemSortField.Total => line => line.Total,
            ReceiptItemSortField.Description => line => line.Description,
            _ => line => line.PurchaseDate,
        };

        IOrderedQueryable<LineRow> sorted = query.Direction == SortDirection.Descending
            ? lines.OrderByDescending(key)
            : lines.OrderBy(key);

        // Stable paging when many lines share the sort key.
        return sorted.ThenBy(line => line.Id);
    }

    private static string EscapeLike(string text) =>
        text.Replace(@"\", @"\\", StringComparison.Ordinal)
            .Replace("%", @"\%", StringComparison.Ordinal)
            .Replace("_", @"\_", StringComparison.Ordinal);

    /// <summary>Query-side shape; EF can filter and sort on its members before the final projection.</summary>
    private sealed class LineRow
    {
        public Guid Id { get; init; }

        public Guid ReceiptId { get; init; }

        public string Store { get; init; } = "";

        public DateOnly PurchaseDate { get; init; }

        public string Item { get; init; } = "";

        public string Category { get; init; } = "";

        public decimal Quantity { get; init; }

        public decimal UnitPrice { get; init; }

        public decimal Amount { get; init; }

        public decimal Discount { get; init; }

        public decimal Total { get; init; }

        public string? Description { get; init; }
    }
}
