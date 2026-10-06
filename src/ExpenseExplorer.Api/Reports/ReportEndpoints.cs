using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Reports;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Reports;

internal static class ReportEndpoints
{
    public static RouteGroupBuilder MapReports(this RouteGroupBuilder api)
    {
        api.MapGet("/reports/categories", CategoriesAsync).WithTags("Reports");
        api.MapGet("/reports/monthly", MonthlyAsync).WithTags("Reports");
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

    /// <summary>The last <paramref name="months"/> calendar months (6 by default), the current one included.</summary>
    private static Task<IResult> MonthlyAsync(
        int? months,
        IReportQueries reports,
        TimeProvider clock,
        CancellationToken cancellationToken) =>
        Input.InRange(months, defaultValue: 6, min: 1, max: 24)
            .ForTarget("months")
            .ToHttpAsync(
                async count => Result.Success(await reports.MonthlyAsync(clock.Today(), count, cancellationToken)),
                report => Results.Ok(report));

    internal static Result<(DateOnly From, DateOnly To)> ParsePeriod(DateOnly? from, DateOnly? to, DateOnly today) =>
        Input.OrderedRange<DateOnly>(from ?? new DateOnly(today.Year, today.Month, 1), to ?? today)
            .ForTarget("from")
            .Map(period => (period.From!.Value, period.To!.Value));
}
