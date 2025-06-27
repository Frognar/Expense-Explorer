using System.Data;
using Dapper;
using DotResult;
using ExpenseExplorer.Application.Features.Categories.GetCategories;
using ExpenseExplorer.Infrastructure.Database;
using static ExpenseExplorer.Infrastructure.Helpers.SqlQueryBuilder;

namespace ExpenseExplorer.Infrastructure.Features.Categories;

internal sealed class CategoryRepository(IDbConnectionFactory connectionFactory)
    : IGetCategoriesPersistence
{
    public async Task<Result<IEnumerable<string>>> GetCategoriesAsync(
        IEnumerable<string> searchTerms,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        string sql = BuildSelectQuery("category", "receipt_items", searchTerms);
        IEnumerable<string> rawData = await connection.QueryAsync<string>(sql);
        return Success.From(rawData.Distinct());
    }
}