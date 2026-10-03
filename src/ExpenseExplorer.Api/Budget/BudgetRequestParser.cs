using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Budget;
using ExpenseExplorer.Contracts.Budget;
using ExpenseExplorer.Domain.Budget;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Budget;

/// <summary>Pure functions from raw budget requests to commands; all problems are reported at once.</summary>
internal static class BudgetRequestParser
{
    public static CreatePeriod ParseCreatePeriod(CreatePeriodRequest? request) => new(request?.Start, request?.End);

    public static Result<Fund> ParseFund(FundRequest request) =>
        ResultCombine.Combine(
            BudgetName.Create(request.Name).ForTarget("name"),
            Input.Required(request.Amount).Bind(FundAmount.Create).ForTarget("amount"),
            (name, amount) => new Fund(name, amount));

    public static Result<PlanItem> ParsePlanItem(PlanItemRequest request) =>
        ResultCombine.Combine(
            Input.Required(request.GroupId).ForTarget("groupId"),
            BudgetName.Create(request.Name).ForTarget("name"),
            Input.Required(request.Amount).Bind(Money.Create).ForTarget("amount"),
            (group, name, amount) => new PlanItem(group, name, amount));

    public static Result<Group> ParseGroup(GroupRequest request) =>
        BudgetName.Create(request.Name).ForTarget("name").Map(name => new Group(name, request.Position ?? 0));

    public static Result<CategoryGroup> ParseCategoryGroup(CategoryGroupRequest request) =>
        CategoryName.Create(request.Category).ForTarget("category").Map(category => new CategoryGroup(category, request.GroupId));

    /// <summary>Errors name the line they are on, e.g. <c>items[2].amount</c>.</summary>
    public static Result<Template> ParseTemplate(TemplateRequest request) =>
        ResultCombine.Combine(
            ResultCombine.Sequence((request.Funds ?? []).Select((fund, index) => ParseFund(fund).ForTargetPrefix($"funds[{index}]"))),
            ResultCombine.Sequence((request.Items ?? []).Select((item, index) => ParseTemplateItem(item).ForTargetPrefix($"items[{index}]"))),
            (funds, items) => new Template(funds, items));

    private static Result<TemplateItem> ParseTemplateItem(TemplateItemRequest request) =>
        ResultCombine.Combine(
            BudgetName.Create(request.Group).ForTarget("group"),
            BudgetName.Create(request.Name).ForTarget("name"),
            Input.Required(request.Amount).Bind(Money.Create).ForTarget("amount"),
            (group, name, amount) => new TemplateItem(group, name, amount));

    private static Result<T> ForTargetPrefix<T>(this Result<T> result, string prefix) =>
        result.Match(
            Result.Success,
            errors => Result.Failure<T>([.. errors.Select(error => error.For($"{prefix}.{error.Target}"))]));
}
