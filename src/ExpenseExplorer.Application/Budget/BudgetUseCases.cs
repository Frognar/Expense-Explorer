using ExpenseExplorer.Domain.Budget;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Application.Budget;

public static class BudgetUseCases
{
    /// <summary>
    /// A period without a start follows the last one; without an end it lasts a month.
    /// Periods never overlap, so every receipt counts in at most one of them.
    /// </summary>
    public static async Task<Result<Guid>> CreatePeriodAsync(
        IBudgetStore store,
        CreatePeriod command,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<(Guid Id, BudgetPeriod Period)> existing = await store.PeriodsAsync(cancellationToken);
        DateOnly? start = command.Start
            ?? (existing.Count == 0 ? null : existing.Max(period => period.Period.End).AddDays(1));
        if (start is not { } first)
        {
            return Result.Failure<Guid>(BudgetErrors.StartRequired);
        }

        Result<BudgetPeriod> period = command.End is { } end
            ? BudgetPeriod.Create(first, end).ForTarget("end")
            : Result.Success(BudgetPeriod.MonthFrom(first));

        Result<BudgetPeriod> free = period.Bind(valid => existing.Any(other => other.Period.Overlaps(valid))
            ? Result.Failure<BudgetPeriod>(BudgetErrors.PeriodOverlaps)
            : Result.Success(valid));

        return await free.Match(
            async valid => Result.Success(await store.CreatePeriodAsync(valid, cancellationToken)),
            errors => Task.FromResult(Result.Failure<Guid>(errors)));
    }

    public static async Task<Result<bool>> DeletePeriodAsync(IBudgetStore store, Guid periodId, CancellationToken cancellationToken) =>
        Found(await store.DeletePeriodAsync(periodId, cancellationToken), BudgetErrors.PeriodNotFound);

    public static async Task<Result<Guid>> AddFundAsync(
        IBudgetStore store,
        Guid periodId,
        Fund fund,
        CancellationToken cancellationToken) =>
        await store.AddFundAsync(periodId, fund, cancellationToken) is { } id
            ? Result.Success(id)
            : Result.Failure<Guid>(BudgetErrors.PeriodNotFound);

    public static async Task<Result<bool>> ChangeFundAsync(
        IBudgetStore store,
        Guid periodId,
        Guid fundId,
        Fund fund,
        CancellationToken cancellationToken) =>
        Found(await store.ChangeFundAsync(periodId, fundId, fund, cancellationToken), BudgetErrors.FundNotFound);

    public static async Task<Result<bool>> RemoveFundAsync(
        IBudgetStore store,
        Guid periodId,
        Guid fundId,
        CancellationToken cancellationToken) =>
        Found(await store.RemoveFundAsync(periodId, fundId, cancellationToken), BudgetErrors.FundNotFound);

    public static async Task<Result<Guid>> AddItemAsync(
        IBudgetStore store,
        Guid periodId,
        PlanItem item,
        CancellationToken cancellationToken)
    {
        if (!await store.PeriodExistsAsync(periodId, cancellationToken))
        {
            return Result.Failure<Guid>(BudgetErrors.PeriodNotFound);
        }

        return await store.GroupExistsAsync(item.GroupId, cancellationToken)
            ? Result.Success(await store.AddItemAsync(periodId, item, cancellationToken))
            : Result.Failure<Guid>(BudgetErrors.UnknownGroup);
    }

    public static async Task<Result<bool>> ChangeItemAsync(
        IBudgetStore store,
        Guid periodId,
        Guid itemId,
        PlanItem item,
        CancellationToken cancellationToken) =>
        await store.GroupExistsAsync(item.GroupId, cancellationToken)
            ? Found(await store.ChangeItemAsync(periodId, itemId, item, cancellationToken), BudgetErrors.ItemNotFound)
            : Result.Failure<bool>(BudgetErrors.UnknownGroup);

    public static async Task<Result<bool>> RemoveItemAsync(
        IBudgetStore store,
        Guid periodId,
        Guid itemId,
        CancellationToken cancellationToken) =>
        Found(await store.RemoveItemAsync(periodId, itemId, cancellationToken), BudgetErrors.ItemNotFound);

    public static async Task<Result<Guid>> AddGroupAsync(IBudgetStore store, Group group, CancellationToken cancellationToken) =>
        await store.GroupNameTakenAsync(group.Name, null, cancellationToken)
            ? Result.Failure<Guid>(BudgetErrors.GroupNameTaken)
            : Result.Success(await store.AddGroupAsync(group, cancellationToken));

    public static async Task<Result<bool>> ChangeGroupAsync(
        IBudgetStore store,
        Guid groupId,
        Group group,
        CancellationToken cancellationToken) =>
        await store.GroupNameTakenAsync(group.Name, groupId, cancellationToken)
            ? Result.Failure<bool>(BudgetErrors.GroupNameTaken)
            : Found(await store.ChangeGroupAsync(groupId, group, cancellationToken), BudgetErrors.GroupNotFound);

    /// <summary>A group with planned expenses, in a period or the template, has to be emptied first.</summary>
    public static async Task<Result<bool>> DeleteGroupAsync(IBudgetStore store, Guid groupId, CancellationToken cancellationToken) =>
        await store.GroupInUseAsync(groupId, cancellationToken)
            ? Result.Failure<bool>(BudgetErrors.GroupInUse)
            : Found(await store.DeleteGroupAsync(groupId, cancellationToken), BudgetErrors.GroupNotFound);

    public static async Task<Result<bool>> SetCategoryGroupAsync(
        IBudgetStore store,
        CategoryGroup assignment,
        CancellationToken cancellationToken)
    {
        if (assignment.GroupId is { } groupId && !await store.GroupExistsAsync(groupId, cancellationToken))
        {
            return Result.Failure<bool>(BudgetErrors.UnknownGroup);
        }

        await store.SetCategoryGroupAsync(assignment, cancellationToken);
        return Result.Success(true);
    }

    public static async Task<Result<bool>> ReplaceTemplateAsync(IBudgetStore store, Template template, CancellationToken cancellationToken)
    {
        await store.ReplaceTemplateAsync(template, cancellationToken);
        return Result.Success(true);
    }

    private static Result<bool> Found(bool found, Error notFound) =>
        found ? Result.Success(true) : Result.Failure<bool>(notFound);
}
