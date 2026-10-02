using System.Globalization;
using System.Net;
using ExpenseExplorer.Api.Logs;
using ExpenseExplorer.Contracts.Auth;
using ExpenseExplorer.Contracts.Logs;
using Microsoft.AspNetCore.Http;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Parsing;

namespace ExpenseExplorer.Api.Tests;

public class LogApiTests(ApiFixture api)
{
    private const string Logs = "/api/v1/logs";

    [Fact]
    public async Task Only_admins_can_read_the_logs()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await api.Anonymous.Get(Logs)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await api.Reader.Get(Logs)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await api.Editor.Get(Logs)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await api.Admin.Get(Logs)).StatusCode);
    }

    [Fact]
    public async Task Admin_can_edit_and_is_told_about_the_log_panel()
    {
        CurrentUserResponse me = await (await api.Admin.Get("/api/v1/auth/me")).Read<CurrentUserResponse>();

        Assert.Equal(new CurrentUserResponse("admin", "Admin", CanEdit: true, CanViewLogs: true), me);
    }

    [Fact]
    public async Task Entries_of_a_day_come_newest_first_with_their_details()
    {
        await WriteLogAsync("20260901",
            """{"@t":"2026-09-01T08:00:00.0000000Z","@m":"Started","@i":"a1b2c3d4","SourceContext":"ExpenseExplorer.Api"}""",
            """{"@t":"2026-09-01T09:30:00.0000000Z","@m":"Receipt failed","@i":"a1b2c3d5","@l":"Error","@x":"System.Exception: boom","SourceContext":"ExpenseExplorer.Api.Receipts","ReceiptId":"42","Lines":[1,2],"@@odd":"x"}""",
            """{"@t":"2026-09-01T09:31:00.00""");

        LogListResponse list = await (await api.Admin.Get($"{Logs}?day=2026-09-01")).Read<LogListResponse>();

        Assert.Equal(new DateOnly(2026, 9, 1), list.Day);
        Assert.Equal(2, list.Entries.TotalCount);
        LogEntryResponse newest = list.Entries.Items[0];
        Assert.Equal(new DateTimeOffset(2026, 9, 1, 9, 30, 0, TimeSpan.Zero), newest.Timestamp);
        Assert.Equal(LogSeverity.Error, newest.Level);
        Assert.Equal("Receipt failed", newest.Message);
        Assert.Equal("ExpenseExplorer.Api.Receipts", newest.Source);
        Assert.Equal("System.Exception: boom", newest.Exception);
        Assert.Equal(
            new Dictionary<string, string> { ["ReceiptId"] = "42", ["Lines"] = "[1,2]", ["@odd"] = "x" },
            newest.Properties);
        Assert.Equal(LogSeverity.Information, list.Entries.Items[1].Level);
    }

    [Fact]
    public async Task Entries_can_be_filtered_by_level_and_text_and_paged()
    {
        await WriteLogAsync("20260902",
            Line("08:00", "Debug", "Cache warmed"),
            Line("08:01", null, "Signed in jan"),
            Line("08:02", "Warning", "Slow query for JAN"),
            Line("08:03", "Error", "Import failed"),
            Line("08:04", "Fatal", "Out of disk"));

        LogListResponse warnings = await (await api.Admin.Get($"{Logs}?day=2026-09-02&level=Warning")).Read<LogListResponse>();
        LogListResponse jan = await (await api.Admin.Get($"{Logs}?day=2026-09-02&search=%20jan%20")).Read<LogListResponse>();
        LogListResponse secondPage = await (await api.Admin.Get($"{Logs}?day=2026-09-02&page=2&pageSize=2")).Read<LogListResponse>();

        Assert.Equal(["Out of disk", "Import failed", "Slow query for JAN"], warnings.Entries.Items.Select(entry => entry.Message));
        Assert.Equal(["Slow query for JAN", "Signed in jan"], jan.Entries.Items.Select(entry => entry.Message));
        Assert.Equal(["Slow query for JAN", "Signed in jan"], secondPage.Entries.Items.Select(entry => entry.Message));
        Assert.Equal(5, secondPage.Entries.TotalCount);
    }

    [Fact]
    public async Task Today_is_shown_by_default_and_every_day_with_a_file_is_listed()
    {
        await WriteLogAsync("20260903", Line("10:00", null, "Earlier day"));
        await WriteLogAsync("20260929", Line("10:00", null, "Today"));
        await File.WriteAllTextAsync(
            Path.Combine(api.App.LogsDirectory, "expense-explorer-20260801.log"), "old text log", TestContext.Current.CancellationToken);

        LogListResponse today = await (await api.Admin.Get(Logs)).Read<LogListResponse>();
        LogListResponse empty = await (await api.Admin.Get($"{Logs}?day=2026-08-15")).Read<LogListResponse>();

        Assert.Equal(ApiFixture.Today, today.Day);
        Assert.Equal(["Today"], today.Entries.Items.Select(entry => entry.Message));
        Assert.Equal(today.Days.OrderDescending(), today.Days);
        Assert.Contains(new DateOnly(2026, 9, 3), today.Days);
        Assert.DoesNotContain(new DateOnly(2026, 8, 1), today.Days);
        Assert.Empty(empty.Entries.Items);
    }

    [Fact]
    public async Task Paging_is_checked()
    {
        HttpResponseMessage response = await api.Admin.Get($"{Logs}?page=0&pageSize=1000");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            new Dictionary<string, string[]> { ["page"] = ["Input.OutOfRange"], ["pageSize"] = ["Input.OutOfRange"] },
            await response.ErrorCodes());
    }

    [Fact]
    public void Lines_written_by_the_configured_formatter_can_be_read_back()
    {
        LogEvent logged = new(
            new DateTimeOffset(2026, 9, 29, 12, 0, 0, TimeSpan.Zero),
            LogEventLevel.Warning,
            new InvalidOperationException("boom"),
            new MessageTemplateParser().Parse("Receipt {ReceiptId} is odd"),
            [
                new LogEventProperty("ReceiptId", new ScalarValue(42)),
                new LogEventProperty("SourceContext", new ScalarValue("ExpenseExplorer.Api")),
            ]);
        using StringWriter line = new(CultureInfo.InvariantCulture);
        new RenderedCompactJsonFormatter().Format(logged, line);

        LogEntryResponse? entry = LogLine.Parse(line.ToString().TrimEnd());

        Assert.NotNull(entry);
        Assert.Equal(logged.Timestamp, entry.Timestamp);
        Assert.Equal(LogSeverity.Warning, entry.Level);
        Assert.Equal("Receipt 42 is odd", entry.Message);
        Assert.Equal("ExpenseExplorer.Api", entry.Source);
        Assert.StartsWith("System.InvalidOperationException: boom", entry.Exception, StringComparison.Ordinal);
        Assert.Equal(new Dictionary<string, string> { ["ReceiptId"] = "42" }, entry.Properties);
    }

    [Theory]
    [InlineData(200, true, null, LogEventLevel.Information)]
    [InlineData(200, false, null, LogEventLevel.Debug)]
    [InlineData(404, false, null, LogEventLevel.Information)]
    [InlineData(500, true, null, LogEventLevel.Error)]
    [InlineData(200, true, "boom", LogEventLevel.Error)]
    public void Requests_for_frontend_files_are_logged_as_debug(int status, bool hasEndpoint, string? exception, LogEventLevel expected)
    {
        DefaultHttpContext http = new();
        http.Response.StatusCode = status;
        if (hasEndpoint)
        {
            http.SetEndpoint(new Endpoint(_ => Task.CompletedTask, null, "api"));
        }

        Assert.Equal(expected, LoggingSetup.RequestLevel(http, 1, exception is null ? null : new InvalidOperationException(exception)));
    }

    [Theory]
    [InlineData("expense-explorer-20261002.clef", "2026-10-02")]
    [InlineData("expense-explorer-20261002_001.clef", "2026-10-02")]
    [InlineData("expense-explorer-20261002.log", null)]
    [InlineData("expense-explorer-20261302.clef", null)]
    public void Day_is_read_from_the_file_name(string fileName, string? day) =>
        Assert.Equal(day is null ? null : DateOnly.Parse(day, CultureInfo.InvariantCulture), LogFileName.DayOf(fileName));

    private Task WriteLogAsync(string day, params string[] lines) =>
        File.WriteAllLinesAsync(
            Path.Combine(api.App.LogsDirectory, $"expense-explorer-{day}.clef"), lines, TestContext.Current.CancellationToken);

    private static string Line(string time, string? level, string message) =>
        $$"""{"@t":"2026-09-02T{{time}}:00.0000000Z","@m":"{{message}}"{{(level is null ? "" : $",\"@l\":\"{level}\"")}}}""";
}
