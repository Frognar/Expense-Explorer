
using System.Collections.Immutable;
using System.Data;
using System.Globalization;
using Dapper;
using DotResult;
using ExpenseExplorer.Application.Features.Reports.GetCategoryReport;
using ExpenseExplorer.Infrastructure.Database;

namespace ExpenseExplorer.Infrastructure.Features.Reports;

internal sealed class ReportRepository(IDbConnectionFactory connectionFactory)
    : IGetCategoryReportPersistence
{
    public async Task<Result<ImmutableList<CategoryExpense>>> GetCategoryReportAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        const string query = """
                             select i.category as "Category", sum(i.quantity * i.unit_price - coalesce(i.discount, 0)) as "Total"
                             from receipts r
                             join receipt_items i on r.id = i.receipt_id
                             where r.purchase_date between @From and @To
                             group by i.category;
                             """;

        try
        {
            using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            var expenses = await connection.QueryAsync<CategoryExpense>(query, new { From = from, To = to });
            return expenses.ToImmutableList();
        }
        catch (Exception ex)
        {
            return Failure.Fatal(
                code: "DB_ERROR",
                message: ex.Message,
                metadata: new Dictionary<string, object>
                {
                    { "StackTrace", ex.StackTrace ?? "" },
                    { "From", from.ToString(CultureInfo.InvariantCulture) },
                    { "To", to.ToString(CultureInfo.InvariantCulture) }
                });
        }
    }
}
