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
        IItemRepository
{
    public async Task<ImmutableArray<Store>> GetStoresAsync(Option<string> search, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
        IEnumerable<string> stores = await search
            .Map(FormatStoreSearchFilters)
            .MatchAsync(
                none: () => connection.QueryAsync<string>("""
                                                          select store
                                                          from receipts
                                                          """),
                some: whereClauses => connection.QueryAsync<string>($"""
                                                                     select store
                                                                     from receipts
                                                                     where {whereClauses}
                                                                     """
                ));

        return Stores
            .CreateMany([..stores.Select(Store.TryCreate)])
            .OrElse(() => []);
    }

    private static string FormatStoreSearchFilters(string searchTerms)
    {
        return string.Join(" AND ", searchTerms
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(str => $"UPPER(store) LIKE '%{str.ToUpperInvariant()}%'"));
    }

    public async Task<ImmutableArray<Item>> GetItemsAsync(Option<string> search, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
        IEnumerable<string> items = await search
            .Map(FormatItemSearchFilters)
            .MatchAsync(
                none: () => connection.QueryAsync<string>("""
                                                          select item
                                                          from receipt_items
                                                          """),
                some: whereClauses => connection.QueryAsync<string>($"""
                                                                     select item
                                                                     from receipt_items
                                                                     where {whereClauses}
                                                                     """
                ));

        return Items
            .CreateMany([..items.Select(Item.TryCreate)])
            .OrElse(() => []);
    }

    private static string FormatItemSearchFilters(string searchTerms)
    {
        return string.Join(" AND ", searchTerms
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(str => $"UPPER(item) LIKE '%{str.ToUpperInvariant()}%'"));
    }
}