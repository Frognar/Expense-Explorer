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
}
