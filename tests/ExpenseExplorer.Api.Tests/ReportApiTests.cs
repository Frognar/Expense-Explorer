using System.Net;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Contracts.Reports;

namespace ExpenseExplorer.Api.Tests;

public class ReportApiTests(ApiFixture api)
{
    private const string Categories = "/api/v1/reports/categories";
    private const string Monthly = "/api/v1/reports/monthly";

    // A period no other test writes to.
    private static readonly DateOnly Start = new(2019, 3, 1);
    private static readonly DateOnly End = new(2019, 3, 31);

    [Fact]
    public async Task Category_report_sums_lines_in_the_period_largest_first()
    {
        await api.Editor.ReceiptAsync(
            "Lidl",
            Start,
            new ReceiptItemRequest("Mleko", "Nabiał", 2m, 6.98m, 0.98m, null),
            new ReceiptItemRequest("Chleb", "Pieczywo", 1m, 4.99m, null, null));
        await api.Editor.ReceiptAsync("Orlen", End, new ReceiptItemRequest("Paliwo", "Transport", 30m, 199.50m, null, null));
        await api.Editor.ReceiptAsync("Orlen", End.AddDays(1), new ReceiptItemRequest("Paliwo", "Transport", 30m, 199.50m, null, null));

        CategoryReportResponse report = await (await api.Reader.Get($"{Categories}?from=2019-03-01&to=2019-03-31"))
            .Read<CategoryReportResponse>();

        Assert.Equal(
            [new("Transport", 199.50m), new("Nabiał", 6.00m), new("Pieczywo", 4.99m)],
            report.Categories);
        Assert.Equal(210.49m, report.Total);
    }

    [Fact]
    public async Task Without_dates_the_report_covers_this_month_up_to_today()
    {
        CategoryReportResponse report = await (await api.Reader.Get(Categories)).Read<CategoryReportResponse>();

        Assert.Equal(new DateOnly(2026, 9, 1), report.From);
        Assert.Equal(ApiFixture.Today, report.To);
    }

    [Fact]
    public async Task Monthly_report_spreads_each_category_over_the_months_up_to_this_one()
    {
        // Other tests write to recent months too, so only this test's own category is checked.
        string category = $"Kategoria-{Guid.NewGuid():N}";
        await api.Editor.ReceiptAsync("Lidl", new DateOnly(2026, 6, 30), new ReceiptItemRequest("Mleko", category, 1m, 10.00m, 1.00m, null));
        await api.Editor.ReceiptAsync("Lidl", new DateOnly(2026, 8, 1), new ReceiptItemRequest("Mleko", category, 2m, 20.00m, null, null));
        await api.Editor.ReceiptAsync("Lidl", new DateOnly(2026, 5, 31), new ReceiptItemRequest("Mleko", category, 1m, 99.00m, null, null));

        MonthlyReportResponse report = await (await api.Reader.Get($"{Monthly}?months=4")).Read<MonthlyReportResponse>();

        Assert.Equal(
            [new DateOnly(2026, 6, 1), new DateOnly(2026, 7, 1), new DateOnly(2026, 8, 1), new DateOnly(2026, 9, 1)],
            report.Months);
        CategoryMonthsResponse mine = Assert.Single(report.Categories, c => c.Category == category);
        Assert.Equal([9.00m, 0m, 20.00m, 0m], mine.Totals);
        Assert.Equal(29.00m, mine.Total);
        Assert.Equal(
            report.Months.Select((_, index) => report.Categories.Sum(c => c.Totals[index])),
            report.Totals);
    }

    [Fact]
    public async Task Monthly_report_covers_at_most_two_years()
    {
        HttpResponseMessage response = await api.Reader.Get($"{Monthly}?months=25");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(["Input.OutOfRange"], (await response.ErrorCodes())["months"]);
    }

    [Fact]
    public async Task Reversed_period_is_rejected()
    {
        HttpResponseMessage response = await api.Reader.Get($"{Categories}?from=2019-04-01&to=2019-03-01");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(["Input.RangeReversed"], (await response.ErrorCodes())["from"]);
    }
}
