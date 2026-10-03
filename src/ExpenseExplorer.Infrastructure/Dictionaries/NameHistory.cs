using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Dictionaries;

internal sealed class NameHistory(ExpenseExplorerDbContext db) : INameHistory
{
    /// <summary>A household has few renames, so all of them are read at once.</summary>
    public async Task<NameAliases> AliasesAsync(CancellationToken cancellationToken)
    {
        List<NameAliasRow> rows = await db.NameAliases.AsNoTracking().ToListAsync(cancellationToken);
        return new NameAliases(rows
            .Select(row => (Kind: NameColumns.Kind(row.Kind), row.Alias, row.Name))
            .Where(row => row.Kind is not null)
            .Select(row => (row.Kind!.Value, row.Alias, row.Name)));
    }

    public async Task<IReadOnlyDictionary<string, string>> LastCategoriesAsync(
        IReadOnlyCollection<string> items,
        CancellationToken cancellationToken)
    {
        if (items.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        string[] names = [.. items];
        var purchases = await (
                from item in db.ReceiptItems.AsNoTracking()
                join receipt in db.Receipts.AsNoTracking() on item.ReceiptId equals receipt.Id
                where names.Contains(item.Item)
                group new { receipt.PurchaseDate, item.Category } by new { item.Item, item.Category } into used
                select new { used.Key.Item, used.Key.Category, LastUsed = used.Max(u => u.PurchaseDate), Uses = used.Count() })
            .ToListAsync(cancellationToken);

        // The latest purchase wins; on the same day the category used more often does.
        return purchases
            .GroupBy(purchase => purchase.Item, StringComparer.Ordinal)
            .ToDictionary(
                item => item.Key,
                item => item
                    .OrderByDescending(purchase => purchase.LastUsed)
                    .ThenByDescending(purchase => purchase.Uses)
                    .ThenBy(purchase => purchase.Category, StringComparer.Ordinal)
                    .First()
                    .Category,
                StringComparer.Ordinal);
    }
}
