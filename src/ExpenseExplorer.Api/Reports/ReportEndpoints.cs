using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Reports;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Reports;

internal static class ReportEndpoints
{
    public static RouteGroupBuilder MapReports(this RouteGroupBuilder api)
    {
        api.MapGet("/reports/categories", CategoriesAsync).WithTags("Reports");
        return api;
    }

    /// <summary>Without dates the report covers the current month up to today.</summary>
    private static Task<IResult> CategoriesAsync(
        DateOnly? from,
        DateOnly? to,
        IReportQueries reports,
        TimeProvider clock,
        CancellationToken cancellationToken) =>
        ParsePeriod(from, to, clock.Today())
            .ToHttpAsync(
                async period => Result.Success(await reports.CategoriesAsync(period.From, period.To, cancellationToken)),
                report => Results.Ok(report));

    internal static Result<(DateOnly From, DateOnly To)> ParsePeriod(DateOnly? from, DateOnly? to, DateOnly today) =>
        Input.OrderedRange<DateOnly>(from ?? new DateOnly(today.Year, today.Month, 1), to ?? today)
            .ForTarget("from")
            .Map(period => (period.From!.Value, period.To!.Value));
}
