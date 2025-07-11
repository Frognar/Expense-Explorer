using System.Data;
using Dapper;
using DotResult;
using ExpenseExplorer.Application.Features.Stores.GetStores;
using ExpenseExplorer.Infrastructure.Database;
using static ExpenseExplorer.Infrastructure.Helpers.SqlQueryBuilder;

namespace ExpenseExplorer.Infrastructure.Features.Stores;

internal sealed class StoreRepository(IDbConnectionFactory connectionFactory)
    : IGetStoresPersistence
{
    public async Task<Result<IEnumerable<string>>> GetStoresAsync(
        IEnumerable<string> searchTerms,
        CancellationToken cancellationToken)
    {
        searchTerms = searchTerms.ToList();

        try
        {
            using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            string sql = BuildSelectQuery("store", "receipts", searchTerms);
            IEnumerable<string> rawData = await connection.QueryAsync<string>(sql);
            return Success.From(rawData.Distinct());
        }
        catch (Exception ex)
        {
            return Failure.Fatal(
                code: ErrorCodes.DbException,
                message: ex.Message,
                metadata: new Dictionary<string, object>
                {
                    { "StackTrace", ex.StackTrace ?? "" },
                    { "SearchTerms", string.Join(", ", searchTerms)}
                });
        }
    }
}