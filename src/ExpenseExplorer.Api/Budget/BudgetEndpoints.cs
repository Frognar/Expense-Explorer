using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Budget;
using ExpenseExplorer.Contracts.Budget;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Budget;

/// <summary>
/// Budget periods with their incomes and planned expenses, budget groups and the template for new
/// periods. Spending is never entered here: it comes from receipts.
/// </summary>
internal static class BudgetEndpoints
{
    public static RouteGroupBuilder MapBudget(this RouteGroupBuilder api)
    {
        RouteGroupBuilder budget = api.MapGroup("/budget").WithTags("Budget");

        budget.MapGet("/periods", async (IBudgetQueries queries, CancellationToken ct) => Results.Ok(await queries.PeriodsAsync(ct)));
        budget.MapGet("/periods/current", CurrentAsync);
        budget.MapGet("/history", HistoryAsync);
        budget.MapGet("/periods/{periodId:guid}", GetAsync);
        budget.MapPost("/periods", CreatePeriodAsync);
        budget.MapDelete("/periods/{periodId:guid}", (Guid periodId, IBudgetStore store, CancellationToken ct) =>
            Done(BudgetUseCases.DeletePeriodAsync(store, periodId, ct)));

        budget.MapPost("/periods/{periodId:guid}/funds", (Guid periodId, FundRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParseFund(request)
                .ToHttpAsync(fund => BudgetUseCases.AddFundAsync(store, periodId, fund, ct), Created(periodId)));
        budget.MapPut("/periods/{periodId:guid}/funds/{fundId:guid}", (Guid periodId, Guid fundId, FundRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParseFund(request)
                .ToHttpAsync(fund => BudgetUseCases.ChangeFundAsync(store, periodId, fundId, fund, ct), _ => Results.NoContent()));
        budget.MapDelete("/periods/{periodId:guid}/funds/{fundId:guid}", (Guid periodId, Guid fundId, IBudgetStore store, CancellationToken ct) =>
            Done(BudgetUseCases.RemoveFundAsync(store, periodId, fundId, ct)));

        budget.MapPost("/periods/{periodId:guid}/items", (Guid periodId, PlanItemRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParsePlanItem(request)
                .ToHttpAsync(item => BudgetUseCases.AddItemAsync(store, periodId, item, ct), Created(periodId)));
        budget.MapPut("/periods/{periodId:guid}/items/{itemId:guid}", (Guid periodId, Guid itemId, PlanItemRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParsePlanItem(request)
                .ToHttpAsync(item => BudgetUseCases.ChangeItemAsync(store, periodId, itemId, item, ct), _ => Results.NoContent()));
        budget.MapDelete("/periods/{periodId:guid}/items/{itemId:guid}", (Guid periodId, Guid itemId, IBudgetStore store, CancellationToken ct) =>
            Done(BudgetUseCases.RemoveItemAsync(store, periodId, itemId, ct)));

        budget.MapGet("/groups", async (IBudgetQueries queries, CancellationToken ct) => Results.Ok(await queries.GroupsAsync(ct)));
        budget.MapPost("/groups", (GroupRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParseGroup(request)
                .ToHttpAsync(group => BudgetUseCases.AddGroupAsync(store, group, ct), id => Results.Created($"{Routing.ApiPrefix}/budget/groups", new CreatedResponse(id))));
        budget.MapPut("/groups/{groupId:guid}", (Guid groupId, GroupRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParseGroup(request)
                .ToHttpAsync(group => BudgetUseCases.ChangeGroupAsync(store, groupId, group, ct), _ => Results.NoContent()));
        budget.MapDelete("/groups/{groupId:guid}", (Guid groupId, IBudgetStore store, CancellationToken ct) =>
            Done(BudgetUseCases.DeleteGroupAsync(store, groupId, ct)));
        budget.MapPut("/categories", (CategoryGroupRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParseCategoryGroup(request)
                .ToHttpAsync(assignment => BudgetUseCases.SetCategoryGroupAsync(store, assignment, ct), _ => Results.NoContent()));

        budget.MapGet("/template", async (IBudgetQueries queries, CancellationToken ct) => Results.Ok(await queries.TemplateAsync(ct)));
        budget.MapPut("/template", (TemplateRequest request, IBudgetStore store, CancellationToken ct) =>
            BudgetRequestParser.ParseTemplate(request)
                .ToHttpAsync(template => BudgetUseCases.ReplaceTemplateAsync(store, template, ct), _ => Results.NoContent()));

        return api;
    }

    private static async Task<IResult> CurrentAsync(IBudgetQueries queries, TimeProvider clock, CancellationToken cancellationToken)
    {
        DateOnly today = clock.Today();
        return await queries.PeriodOnAsync(today, cancellationToken) is { } periodId
               && await queries.GetAsync(periodId, today, cancellationToken) is { } budget
            ? Results.Ok(budget)
            : ErrorResults.From([BudgetErrors.NoCurrentPeriod]);
    }

    /// <summary>The last <paramref name="count"/> periods (6 by default) that have started, oldest first.</summary>
    private static Task<IResult> HistoryAsync(int? count, IBudgetQueries queries, TimeProvider clock, CancellationToken cancellationToken) =>
        Input.InRange(count, defaultValue: 6, min: 1, max: 24)
            .ForTarget("count")
            .ToHttpAsync(
                async periods => Result.Success(await queries.HistoryAsync(periods, clock.Today(), cancellationToken)),
                history => Results.Ok(history));

    private static async Task<IResult> GetAsync(Guid periodId, IBudgetQueries queries, TimeProvider clock, CancellationToken cancellationToken) =>
        await queries.GetAsync(periodId, clock.Today(), cancellationToken) is { } budget
            ? Results.Ok(budget)
            : ErrorResults.From([BudgetErrors.PeriodNotFound]);

    private static async Task<IResult> CreatePeriodAsync(
        CreatePeriodRequest? request,
        IBudgetStore store,
        IBudgetQueries queries,
        TimeProvider clock,
        CancellationToken cancellationToken)
    {
        Result<Guid> created = await BudgetUseCases.CreatePeriodAsync(store, BudgetRequestParser.ParseCreatePeriod(request), cancellationToken);
        return await created.Match<Task<IResult>>(
            async periodId => Results.Created(
                $"{Routing.ApiPrefix}/budget/periods/{periodId}",
                await queries.GetAsync(periodId, clock.Today(), cancellationToken)),
            errors => Task.FromResult(ErrorResults.From(errors)));
    }

    private static Func<Guid, IResult> Created(Guid periodId) =>
        id => Results.Created($"{Routing.ApiPrefix}/budget/periods/{periodId}", new CreatedResponse(id));

    private static async Task<IResult> Done(Task<Result<bool>> change) =>
        (await change).ToHttp(_ => Results.NoContent());
}
