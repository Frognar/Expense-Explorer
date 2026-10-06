using System.Net;
using ExpenseExplorer.Contracts.Budget;
using ExpenseExplorer.Contracts.Dictionaries;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Domain.Users;

namespace ExpenseExplorer.Api.Tests;

/// <summary>The template, groups and categories are shared by the whole app, so every test gets its own database.</summary>
public sealed class BudgetApiTests(ApiFixture api) : IAsyncLifetime
{
    private const string Budget = "/api/v1/budget";

    private static readonly TemplateRequest Template = new(
        [new FundRequest("Wypłata", 10_000m), new FundRequest("Oszczędności", -2_000m)],
        [
            new TemplateItemRequest("Dom", "Prąd", 300m),
            new TemplateItemRequest("Dom", "Kredyt", 3_000m),
            new TemplateItemRequest("Jedzenie", "Spożywcze", 2_000m),
        ]);

    private ApiFactory _app = null!;
    private HttpClient _editor = null!;

    public async ValueTask InitializeAsync()
    {
        _app = new ApiFactory(await api.CreateDatabaseAsync($"budget_{Guid.NewGuid():N}"));
        _editor = _app.CreateClient().WithAccessToken(await _app.SignInAsync(UserRole.Editor));
    }

    public async ValueTask DisposeAsync()
    {
        _editor.Dispose();
        await _app.DisposeAsync();
    }

    [Fact]
    public async Task New_period_starts_from_the_template_and_counts_spending_from_receipts()
    {
        Assert.Equal(HttpStatusCode.NoContent, (await _editor.Put($"{Budget}/template", Template)).StatusCode);
        IReadOnlyList<GroupResponse> groups = await _editor.Get($"{Budget}/groups").Then<List<GroupResponse>>();
        Guid home = groups.Single(group => group.Name == "Dom").Id;
        await AssignAsync("Media", home);
        await AssignAsync("Kredyt", home);
        await AssignAsync("Spożywcze", groups.Single(group => group.Name == "Jedzenie").Id);

        await _editor.ReceiptAsync("Tauron", new DateOnly(2026, 9, 10), Line("Prąd", "Media", 280m));
        await _editor.ReceiptAsync("Bank", new DateOnly(2026, 9, 15), Line("Rata", "Kredyt", 3_100m));
        await _editor.ReceiptAsync("Lidl", new DateOnly(2026, 9, 20), Line("Chleb", "Spożywcze", 500m), Line("Prezent", "Prezenty", 120m));
        await _editor.ReceiptAsync("Lidl", new DateOnly(2026, 9, 4), Line("Chleb", "Spożywcze", 999m));

        HttpResponseMessage response = await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(new DateOnly(2026, 9, 5), null));
        BudgetResponse budget = await response.Read<BudgetResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal((new DateOnly(2026, 9, 5), new DateOnly(2026, 10, 4)), (budget.Period.Start, budget.Period.End));
        Assert.Equal(["Wypłata", "Oszczędności"], budget.Funds.Select(fund => fund.Name));
        Assert.Equal(
            [("Dom", 3_300m, 3_380m, -80m, 2), ("Jedzenie", 2_000m, 500m, 1_500m, 1)],
            budget.Groups.Select(group => (group.Name, group.Planned, group.Spent, group.Remaining, group.Items.Count)));
        Assert.Equal([new CategorySpentResponse("Prezenty", 120m)], budget.Unassigned);
        Assert.Equal(8_000m, budget.TotalFunds);
        Assert.Equal(4_000m, budget.Spent);
        Assert.Equal(8_000m - 3_380m - 2_000m - 120m, budget.FreePool);
        Assert.Equal(6, budget.DaysLeft);
        BudgetResponse current = await _editor.Get($"{Budget}/periods/current").Then<BudgetResponse>();
        Assert.Equal((budget.Period, budget.FreePool), (current.Period, current.FreePool));
    }

    [Fact]
    public async Task Next_period_follows_the_last_one_and_periods_never_overlap()
    {
        await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(new DateOnly(2026, 9, 5), null));

        BudgetResponse next = await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(null, null)).Then<BudgetResponse>();
        HttpResponseMessage overlapping = await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(new DateOnly(2026, 10, 1), null));

        Assert.Equal((new DateOnly(2026, 10, 5), new DateOnly(2026, 11, 4)), (next.Period.Start, next.Period.End));
        Assert.Equal(HttpStatusCode.Conflict, overlapping.StatusCode);
        Assert.Equal(
            [new DateOnly(2026, 10, 5), new DateOnly(2026, 9, 5)],
            (await _editor.Get($"{Budget}/periods").Then<List<PeriodResponse>>()).Select(period => period.Start));
    }

    [Fact]
    public async Task First_period_needs_a_start_and_there_is_no_current_one_before_it()
    {
        HttpResponseMessage response = await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(null, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Input.Required", (await response.ErrorCodes())["start"]);
        Assert.Equal(HttpStatusCode.NotFound, (await _editor.Get($"{Budget}/periods/current")).StatusCode);
    }

    [Fact]
    public async Task Funds_can_be_corrected_during_the_period()
    {
        BudgetResponse budget = await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(new DateOnly(2026, 9, 5), null)).Then<BudgetResponse>();
        string funds = $"{Budget}/periods/{budget.Period.Id}/funds";

        (await _editor.Post(funds, new FundRequest("Wypłata", 10_000m))).EnsureSuccessStatusCode();
        (await _editor.Post(funds, new FundRequest("Urodziny", 300m))).EnsureSuccessStatusCode();
        FundResponse leave = (await Reload(budget)).Funds[1];
        (await _editor.Put($"{funds}/{leave.Id}", new FundRequest("Wolne w pracy", -1_200m))).EnsureSuccessStatusCode();
        HttpResponseMessage invalid = await _editor.Post(funds, new FundRequest(" ", 0m));

        Assert.Equal(8_800m, (await Reload(budget)).TotalFunds);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Dictionary<string, string[]> errors = await invalid.ErrorCodes();
        Assert.Equal(["BudgetName.Empty", "FundAmount.Zero"], [.. errors["name"], .. errors["amount"]]);

        Assert.Equal(HttpStatusCode.NoContent, (await _editor.Delete($"{funds}/{leave.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _editor.Delete($"{funds}/{leave.Id}")).StatusCode);
        Assert.Equal(10_000m, (await Reload(budget)).TotalFunds);
    }

    [Fact]
    public async Task Planned_expenses_belong_to_existing_groups()
    {
        BudgetResponse budget = await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(new DateOnly(2026, 9, 5), null)).Then<BudgetResponse>();
        string items = $"{Budget}/periods/{budget.Period.Id}/items";
        Guid group = await AddGroupAsync("Dzieci");

        HttpResponseMessage unknown = await _editor.Post(items, new PlanItemRequest(Guid.NewGuid(), "Przedszkole", 488m));
        (await _editor.Post(items, new PlanItemRequest(group, "Przedszkole", 488m))).EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.BadRequest, unknown.StatusCode);
        Assert.Contains("Budget.GroupNotFound", (await unknown.ErrorCodes())["groupId"]);
        PlanItemResponse item = Assert.Single((await Reload(budget)).Groups.Single(g => g.Id == group).Items);
        Assert.Equal(("Przedszkole", 488m), (item.Name, item.Amount));

        Assert.Equal(HttpStatusCode.Conflict, (await _editor.Delete($"{Budget}/groups/{group}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await _editor.Delete($"{items}/{item.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await _editor.Delete($"{Budget}/groups/{group}")).StatusCode);
    }

    [Fact]
    public async Task Group_names_are_unique()
    {
        await AddGroupAsync("Auto");

        HttpResponseMessage response = await _editor.Post($"{Budget}/groups", new GroupRequest("Auto", 1));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Renamed_category_stays_in_its_group()
    {
        Guid food = await AddGroupAsync("Jedzenie");
        await AssignAsync("Nabial", food);
        await _editor.ReceiptAsync("Dino", new DateOnly(2026, 9, 20), Line("Mleko", "Nabial", 4m));

        (await _editor.Post("/api/v1/dictionaries/categories/rename", new RenameNameRequest("Nabial", "Nabiał"))).EnsureSuccessStatusCode();

        GroupResponse group = Assert.Single(await _editor.Get($"{Budget}/groups").Then<List<GroupResponse>>());
        Assert.Equal(["Nabiał"], group.Categories);
    }

    [Fact]
    public async Task Template_problems_name_the_line()
    {
        HttpResponseMessage response = await _editor.Put(
            $"{Budget}/template",
            new TemplateRequest([new FundRequest("Wypłata", null)], [new TemplateItemRequest("Dom", "Prąd", -1m)]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Dictionary<string, string[]> errors = await response.ErrorCodes();
        Assert.Contains("Input.Required", errors["funds[0].amount"]);
        Assert.Contains("Money.Negative", errors["items[0].amount"]);
    }

    [Fact]
    public async Task Only_editors_see_the_budget()
    {
        HttpClient reader = _app.CreateClient().WithAccessToken(await _app.SignInAsync(UserRole.Reader));

        Assert.Equal(HttpStatusCode.Forbidden, (await reader.Get($"{Budget}/periods")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _app.CreateClient().Get($"{Budget}/periods")).StatusCode);
    }

    [Fact]
    public async Task History_lists_started_periods_oldest_first_with_plan_and_spending_per_group()
    {
        Assert.Equal(HttpStatusCode.NoContent, (await _editor.Put($"{Budget}/template", Template)).StatusCode);
        IReadOnlyList<GroupResponse> groups = await _editor.Get($"{Budget}/groups").Then<List<GroupResponse>>();
        await AssignAsync("Spożywcze", groups.Single(group => group.Name == "Jedzenie").Id);
        await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(new DateOnly(2026, 7, 5), null));
        await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(null, null));
        await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(null, null));
        await _editor.Post($"{Budget}/periods", new CreatePeriodRequest(null, null));
        await _editor.ReceiptAsync("Lidl", new DateOnly(2026, 8, 10), Line("Chleb", "Spożywcze", 2_100m), Line("Prezent", "Prezenty", 50m));
        await _editor.ReceiptAsync("Lidl", new DateOnly(2026, 9, 10), Line("Chleb", "Spożywcze", 1_500m));

        List<PeriodResultResponse> history = await _editor.Get($"{Budget}/history?count=2").Then<List<PeriodResultResponse>>();

        Assert.Equal([new DateOnly(2026, 8, 5), new DateOnly(2026, 9, 5)], history.Select(period => period.Period.Start));
        Assert.Equal(
            [("Dom", 3_300m, 0m), ("Jedzenie", 2_000m, 2_100m)],
            history[0].Groups.Select(group => (group.Name, group.Planned, group.Spent)));
        Assert.Equal((50m, 2_150m, 8_000m - 3_300m - 2_100m - 50m), (history[0].OutsideGroups, history[0].Spent, history[0].FreePool));
        Assert.Equal(1_500m, history[1].Groups.Single(group => group.Name == "Jedzenie").Spent);
    }

    private static ReceiptItemRequest Line(string item, string category, decimal amount) => new(item, category, 1m, amount, null, null);

    private async Task AssignAsync(string category, Guid groupId) =>
        (await _editor.Put($"{Budget}/categories", new CategoryGroupRequest(category, groupId))).EnsureSuccessStatusCode();

    private async Task<Guid> AddGroupAsync(string name)
    {
        await (await _editor.Post($"{Budget}/groups", new GroupRequest(name, 0))).Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return (await _editor.Get($"{Budget}/groups").Then<List<GroupResponse>>()).Single(group => group.Name == name).Id;
    }

    private Task<BudgetResponse> Reload(BudgetResponse budget) =>
        _editor.Get($"{Budget}/periods/{budget.Period.Id}").Then<BudgetResponse>();
}

internal static class ResponseChain
{
    public static async Task<T> Then<T>(this Task<HttpResponseMessage> response) => await (await response).Read<T>();
}
