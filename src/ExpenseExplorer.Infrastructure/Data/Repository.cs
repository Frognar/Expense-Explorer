using System.Collections.Immutable;
using System.Data;
using Dapper;
using DotMaybe;
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
    public async Task<ImmutableArray<Store>> GetStoresAsync(
        Maybe<string> search,
        CancellationToken cancellationToken)
    {
        string sql = SqlQueryBuilder.BuildSelectQuery("store", "receipts", search);
        return await FetchEntitiesAsync(sql, Store.TryCreate, cancellationToken);
    }

    public async Task<ImmutableArray<Item>> GetItemsAsync(
        Maybe<string> search,
        CancellationToken cancellationToken)
    {
        string sql = SqlQueryBuilder.BuildSelectQuery("item", "receipt_items", search);
        return await FetchEntitiesAsync(sql, Item.TryCreate, cancellationToken);
    }

    public async Task<ImmutableArray<Category>> GetCategoriesAsync(
        Maybe<string> search,
        CancellationToken cancellationToken)
    {
        string sql = SqlQueryBuilder.BuildSelectQuery("category", "receipt_items", search);
        return await FetchEntitiesAsync(sql, Category.TryCreate, cancellationToken);
    }

    private async Task<ImmutableArray<T>> FetchEntitiesAsync<T>(
        string sql,
        Func<string, Maybe<T>> entityFactory,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        IEnumerable<string> rawData = await connection.QueryAsync<string>(sql);
        return rawData
            .Distinct()
            .Select(entityFactory)
            .TraverseMaybeToImmutableArray()
            .OrDefault(() => []);
    }

    private static class SqlQueryBuilder
    {
        public static string BuildSelectQuery(
            string columnName,
            string tableName,
            Maybe<string> searchTerms) =>
            searchTerms
                .Map(terms => BuildWhereClause(terms, columnName))
                .Map(whereClause => $"select {columnName} from {tableName} where {whereClause}")
                .OrDefault(() => $"select {columnName} from {tableName}");

        private static string BuildWhereClause(string searchTerms, string columnName) =>
            string.Join(" AND ",
                ParseSearchTerms(searchTerms)
                    .Select(term => CreateLikeCondition(columnName, term)));

        private static string[] ParseSearchTerms(string searchTerms) =>
            searchTerms.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private static string CreateLikeCondition(string columnName, string searchTerm) =>
            $"UPPER({columnName}) LIKE '%{searchTerm.ToUpperInvariant()}%'";
    }
}