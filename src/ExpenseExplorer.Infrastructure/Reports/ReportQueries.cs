using ExpenseExplorer.Application.Reports;
using ExpenseExplorer.Contracts.Reports;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Reports;

internal sealed class ReportQueries(ExpenseExplorerDbContext db) : IReportQueries
{
    public async Task<CategoryReportResponse> CategoriesAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken)
    {
        List<CategoryExpenseResponse> categories = await (
                from item in db.ReceiptItems.AsNoTracking()
                join receipt in db.Receipts.AsNoTracking() on item.ReceiptId equals receipt.Id
                where receipt.PurchaseDate >= start && receipt.PurchaseDate <= end
                group item by item.Category into category
                select new CategoryExpenseResponse(category.Key, category.Sum(i => i.Amount - i.Discount)))
            .ToListAsync(cancellationToken);

        return new CategoryReportResponse(
            start,
            end,
            [.. categories.OrderByDescending(c => c.Total).ThenBy(c => c.Category, StringComparer.Ordinal)],
            categories.Sum(c => c.Total));
    }

    public async Task<MonthlyReportResponse> MonthlyAsync(DateOnly lastDay, int count, CancellationToken cancellationToken)
    {
        DateOnly lastMonth = new(lastDay.Year, lastDay.Month, 1);
        DateOnly start = lastMonth.AddMonths(1 - count);
        DateOnly end = lastMonth.AddMonths(1).AddDays(-1);
        var spent = await (
                from item in db.ReceiptItems.AsNoTracking()
                join receipt in db.Receipts.AsNoTracking() on item.ReceiptId equals receipt.Id
                where receipt.PurchaseDate >= start && receipt.PurchaseDate <= end
                group item by new { item.Category, receipt.PurchaseDate.Year, receipt.PurchaseDate.Month } into month
                select new { month.Key.Category, month.Key.Year, month.Key.Month, Total = month.Sum(i => i.Amount - i.Discount) })
            .ToListAsync(cancellationToken);

        DateOnly[] months = [.. Enumerable.Range(0, count).Select(start.AddMonths)];
        List<CategoryMonthsResponse> categories =
        [
            .. spent
                .GroupBy(row => row.Category, StringComparer.Ordinal)
                .Select(category =>
                {
                    decimal[] totals = [.. months.Select(month => category
                        .Where(row => row.Year == month.Year && row.Month == month.Month)
                        .Sum(row => row.Total))];
                    return new CategoryMonthsResponse(category.Key, totals, totals.Sum());
                })
                .OrderByDescending(category => category.Total)
                .ThenBy(category => category.Category, StringComparer.Ordinal),
        ];

        return new MonthlyReportResponse(
            months,
            [.. months.Select((_, index) => categories.Sum(category => category.Totals[index]))],
            categories);
    }
}
