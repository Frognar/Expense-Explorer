using System.Data;
using Dapper;
using DotResult;
using ExpenseExplorer.Application.Features.Items.GetItems;
using ExpenseExplorer.Infrastructure.Database;
using static ExpenseExplorer.Infrastructure.Helpers.SqlQueryBuilder;

namespace ExpenseExplorer.Infrastructure.Features.Items;

internal sealed class ItemRepository(IDbConnectionFactory connectionFactory)
    : IGetItemsPersistence
{
    public async Task<Result<IEnumerable<string>>> GetItemsAsync(
        IEnumerable<string> searchTerms,
        CancellationToken cancellationToken)
    {
        searchTerms = searchTerms.ToList();

        try
        {
            using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            string sql = BuildSelectQuery("item", "receipt_items", searchTerms);
            IEnumerable<string> rawData = await connection.QueryAsync<string>(sql);
            return Success.From(rawData.Distinct());
        }
        catch (Exception ex)
        {
            return Failure.Fatal(
                code: "DB_EXCEPTION",
                message: ex.Message,
                metadata: new Dictionary<string, object>
                {
                    { "StackTrace", ex.StackTrace ?? "" },
                    { "SearchTerms", string.Join(", ", searchTerms)}
                });
        }
    }
}