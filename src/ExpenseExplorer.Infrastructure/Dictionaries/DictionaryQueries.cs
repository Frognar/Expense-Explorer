using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Dictionaries;

internal sealed class DictionaryQueries(ExpenseExplorerDbContext db) : IDictionaryQueries
{
    public Task<IReadOnlyList<string>> StoresAsync(DictionaryQuery query, CancellationToken cancellationToken) =>
        DistinctAsync(db.Receipts.Select(r => r.Store), query, cancellationToken);

    public Task<IReadOnlyList<string>> ItemsAsync(DictionaryQuery query, CancellationToken cancellationToken) =>
        DistinctAsync(db.ReceiptItems.Select(i => i.Item), query, cancellationToken);

    public Task<IReadOnlyList<string>> CategoriesAsync(DictionaryQuery query, CancellationToken cancellationToken) =>
        DistinctAsync(db.ReceiptItems.Select(i => i.Category), query, cancellationToken);

    private static async Task<IReadOnlyList<string>> DistinctAsync(
        IQueryable<string> names,
        DictionaryQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Search.Length > 0)
        {
            string pattern = $"%{EscapeLike(query.Search)}%";
            names = names.Where(name => EF.Functions.ILike(name, pattern, @"\"));
        }

        return await names
            .Distinct()
            .OrderBy(name => name)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);
    }

    private static string EscapeLike(string value) =>
        value.Replace(@"\", @"\\", StringComparison.Ordinal)
            .Replace("%", @"\%", StringComparison.Ordinal)
            .Replace("_", @"\_", StringComparison.Ordinal);
}
