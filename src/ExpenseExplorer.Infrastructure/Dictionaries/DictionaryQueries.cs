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
        TextMatch match = await TextSearch.MatchAsync(names, query.Search, cancellationToken);
        IQueryable<string> distinct = match.Filter(names, name => name).Distinct();
        return await match.BestFirst(distinct, name => name)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);
    }
}
