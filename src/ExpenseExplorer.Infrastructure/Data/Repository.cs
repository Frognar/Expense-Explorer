using System.Collections.Immutable;
using System.Data;
using Dapper;
using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;
using ExpenseExplorer.Infrastructure.Database;

namespace ExpenseExplorer.Infrastructure.Data;

internal sealed class Repository(IDbConnectionFactory connectionFactory)
    : IStoreRepository,
        IItemRepository,
        ICategoryRepository
{
    public async Task<ImmutableArray<Store>> GetStoresAsync(Option<string> search, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
        string sql = "select store from receipts"
                     + search
                         .Map(FormatSearchFilters("store"))
                         .Map(whereClauses => $" where {whereClauses}")
                         .OrElse(() => "");

        IEnumerable<string> stores = await connection.QueryAsync<string>(sql);
        return stores.Select(Store.TryCreate)
            .TraverseOptionToImmutableArray()
            .OrElse(() => []);
    }

    public async Task<ImmutableArray<Item>> GetItemsAsync(Option<string> search, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
        string sql = "select item from receipt_items"
                     + search
                         .Map(FormatSearchFilters("item"))
                         .Map(whereClauses => $" where {whereClauses}")
                         .OrElse(() => "");

        IEnumerable<string> items = await connection.QueryAsync<string>(sql);
        return items.Select(Item.TryCreate)
            .TraverseOptionToImmutableArray()
            .OrElse(() => []);
    }

    public async Task<ImmutableArray<Category>> GetCategoriesAsync(
        Option<string> search,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
        string sql = "select category from receipt_items"
                     + search
                         .Map(FormatSearchFilters("category"))
                         .Map(whereClauses => $" where {whereClauses}")
                         .OrElse(() => "");

        IEnumerable<string> categories = await connection.QueryAsync<string>(sql);

        return categories.Select(Category.TryCreate)
            .TraverseOptionToImmutableArray()
            .OrElse(() => []);
    }

    private static Func<string, string> FormatSearchFilters(string column) =>
        searchTerms => FormatSearchFilters(searchTerms, column);

    private static string FormatSearchFilters(string searchTerms, string column) =>
        string.Join(" AND ", searchTerms
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(str => $"UPPER({column}) LIKE '%{str.ToUpperInvariant()}%'"));
}