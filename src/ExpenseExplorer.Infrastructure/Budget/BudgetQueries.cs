using ExpenseExplorer.Application.Budget;
using ExpenseExplorer.Contracts.Budget;
using ExpenseExplorer.Domain.Budget;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Budget;

internal sealed class BudgetQueries(ExpenseExplorerDbContext db) : IBudgetQueries
{
    public async Task<IReadOnlyList<PeriodResponse>> PeriodsAsync(CancellationToken cancellationToken) =>
        await db.BudgetPeriods.AsNoTracking()
            .OrderByDescending(p => p.Start)
            .Select(p => new PeriodResponse(p.Id, p.Start, p.End))
            .ToListAsync(cancellationToken);

    public async Task<Guid?> PeriodOnAsync(DateOnly day, CancellationToken cancellationToken) =>
        await db.BudgetPeriods.AsNoTracking()
            .Where(p => p.Start <= day && p.End >= day)
            .Select(p => (Guid?)p.Id)
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<BudgetResponse?> GetAsync(Guid periodId, DateOnly today, CancellationToken cancellationToken)
    {
        BudgetPeriodRow? row = await db.BudgetPeriods.AsNoTracking().SingleOrDefaultAsync(p => p.Id == periodId, cancellationToken);
        if (row is null || BudgetPeriod.Create(row.Start, row.End).Match<BudgetPeriod?>(valid => valid, _ => null) is not { } period)
        {
            return null;
        }

        List<FundResponse> funds = await db.BudgetFunds.AsNoTracking()
            .Where(f => f.PeriodId == periodId)
            .OrderBy(f => f.Position)
            .Select(f => new FundResponse(f.Id, f.Name, f.Amount))
            .ToListAsync(cancellationToken);
        List<PlanItemResponse> items = await db.BudgetItems.AsNoTracking()
            .Where(i => i.PeriodId == periodId)
            .OrderBy(i => i.Position)
            .Select(i => new PlanItemResponse(i.Id, i.GroupId, i.Name, i.Amount))
            .ToListAsync(cancellationToken);
        IReadOnlyList<GroupResponse> groups = await GroupsAsync(cancellationToken);
        Dictionary<string, decimal> spent = await SpentByCategoryAsync(period, cancellationToken);

        BudgetSummary summary = BudgetSummary.Calculate(
            period,
            today,
            funds.Sum(fund => fund.Amount),
            [.. groups.Select(group => new GroupPlan(
                group.Id,
                group.Name,
                items.Where(item => item.GroupId == group.Id).Sum(item => item.Amount),
                group.Categories))],
            spent);

        return new BudgetResponse(
            new PeriodResponse(row.Id, period.Start, period.End),
            funds,
            [.. summary.Groups.Zip(groups, (result, group) => new GroupBudgetResponse(
                group.Id,
                group.Name,
                result.Planned,
                result.Spent,
                result.Remaining,
                group.Categories,
                [.. items.Where(item => item.GroupId == group.Id)]))],
            [.. summary.Unassigned
                .OrderByDescending(category => category.Value)
                .Select(category => new CategorySpentResponse(category.Key, category.Value))],
            summary.Funds,
            summary.Planned,
            summary.Spent,
            summary.FreePool,
            summary.DaysLeft,
            summary.PerDay);
    }

    public async Task<IReadOnlyList<GroupResponse>> GroupsAsync(CancellationToken cancellationToken)
    {
        List<BudgetGroupRow> groups = await db.BudgetGroups.AsNoTracking().ToListAsync(cancellationToken);
        List<BudgetCategoryRow> categories = await db.BudgetCategories.AsNoTracking().ToListAsync(cancellationToken);
        return
        [
            .. groups
                .OrderBy(group => group.Position)
                .ThenBy(group => group.Name, StringComparer.Ordinal)
                .Select(group => new GroupResponse(
                    group.Id,
                    group.Name,
                    group.Position,
                    [.. categories
                        .Where(category => category.GroupId == group.Id)
                        .Select(category => category.Category)
                        .Order(StringComparer.Ordinal)])),
        ];
    }

    public async Task<TemplateResponse> TemplateAsync(CancellationToken cancellationToken)
    {
        List<FundResponse> funds = await db.BudgetFunds.AsNoTracking()
            .Where(f => f.PeriodId == null)
            .OrderBy(f => f.Position)
            .Select(f => new FundResponse(f.Id, f.Name, f.Amount))
            .ToListAsync(cancellationToken);
        List<TemplateItemResponse> items = await (
                from item in db.BudgetItems.AsNoTracking()
                join grp in db.BudgetGroups.AsNoTracking() on item.GroupId equals grp.Id
                where item.PeriodId == null
                orderby item.Position
                select new TemplateItemResponse(item.Id, grp.Name, item.Name, item.Amount))
            .ToListAsync(cancellationToken);
        return new TemplateResponse(funds, items);
    }

    /// <summary>What receipts dated within the period cost, per category, after discounts.</summary>
    private async Task<Dictionary<string, decimal>> SpentByCategoryAsync(BudgetPeriod period, CancellationToken cancellationToken) =>
        await (
                from item in db.ReceiptItems.AsNoTracking()
                join receipt in db.Receipts.AsNoTracking() on item.ReceiptId equals receipt.Id
                where receipt.PurchaseDate >= period.Start && receipt.PurchaseDate <= period.End
                group item by item.Category into category
                select new { Category = category.Key, Spent = category.Sum(i => i.Amount - i.Discount) })
            .ToDictionaryAsync(category => category.Category, category => category.Spent, StringComparer.Ordinal, cancellationToken);
}
