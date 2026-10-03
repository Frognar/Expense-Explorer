using ExpenseExplorer.Application.Budget;
using ExpenseExplorer.Domain.Budget;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpenseExplorer.Infrastructure.Budget;

internal sealed class BudgetStore(ExpenseExplorerDbContext db) : IBudgetStore
{
    public async Task<IReadOnlyList<(Guid Id, BudgetPeriod Period)>> PeriodsAsync(CancellationToken cancellationToken)
    {
        List<BudgetPeriodRow> rows = await db.BudgetPeriods.AsNoTracking().ToListAsync(cancellationToken);
        return [.. rows.Select(row => (row.Id, ToDomain(row)))];
    }

    public async Task<Guid> CreatePeriodAsync(BudgetPeriod period, CancellationToken cancellationToken)
    {
        BudgetPeriodRow row = new() { Id = Guid.CreateVersion7(), Start = period.Start, End = period.End };
        db.BudgetPeriods.Add(row);

        List<BudgetFundRow> funds = await db.BudgetFunds.AsNoTracking().Where(f => f.PeriodId == null).ToListAsync(cancellationToken);
        db.BudgetFunds.AddRange(funds.Select(fund => new BudgetFundRow
        {
            Id = Guid.CreateVersion7(),
            PeriodId = row.Id,
            Position = fund.Position,
            Name = fund.Name,
            Amount = fund.Amount,
        }));

        List<BudgetItemRow> items = await db.BudgetItems.AsNoTracking().Where(i => i.PeriodId == null).ToListAsync(cancellationToken);
        db.BudgetItems.AddRange(items.Select(item => new BudgetItemRow
        {
            Id = Guid.CreateVersion7(),
            PeriodId = row.Id,
            GroupId = item.GroupId,
            Position = item.Position,
            Name = item.Name,
            Amount = item.Amount,
        }));

        await db.SaveChangesAsync(cancellationToken);
        return row.Id;
    }

    public async Task<bool> DeletePeriodAsync(Guid periodId, CancellationToken cancellationToken) =>
        await db.BudgetPeriods.Where(p => p.Id == periodId).ExecuteDeleteAsync(cancellationToken) > 0;

    public Task<bool> PeriodExistsAsync(Guid periodId, CancellationToken cancellationToken) =>
        db.BudgetPeriods.AnyAsync(p => p.Id == periodId, cancellationToken);

    public async Task<Guid?> AddFundAsync(Guid periodId, Fund fund, CancellationToken cancellationToken)
    {
        if (!await PeriodExistsAsync(periodId, cancellationToken))
        {
            return null;
        }

        BudgetFundRow row = new()
        {
            Id = Guid.CreateVersion7(),
            PeriodId = periodId,
            Position = await NextPositionAsync(db.BudgetFunds.Where(f => f.PeriodId == periodId).Select(f => f.Position), cancellationToken),
        };
        CopyTo(fund, row);
        db.BudgetFunds.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return row.Id;
    }

    public async Task<bool> ChangeFundAsync(Guid periodId, Guid fundId, Fund fund, CancellationToken cancellationToken)
    {
        BudgetFundRow? row = await db.BudgetFunds.SingleOrDefaultAsync(f => f.Id == fundId && f.PeriodId == periodId, cancellationToken);
        if (row is null)
        {
            return false;
        }

        CopyTo(fund, row);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveFundAsync(Guid periodId, Guid fundId, CancellationToken cancellationToken) =>
        await db.BudgetFunds.Where(f => f.Id == fundId && f.PeriodId == periodId).ExecuteDeleteAsync(cancellationToken) > 0;

    public Task<bool> GroupExistsAsync(Guid groupId, CancellationToken cancellationToken) =>
        db.BudgetGroups.AnyAsync(g => g.Id == groupId, cancellationToken);

    public async Task<Guid> AddItemAsync(Guid periodId, PlanItem item, CancellationToken cancellationToken)
    {
        BudgetItemRow row = new()
        {
            Id = Guid.CreateVersion7(),
            PeriodId = periodId,
            Position = await NextPositionAsync(db.BudgetItems.Where(i => i.PeriodId == periodId).Select(i => i.Position), cancellationToken),
        };
        CopyTo(item, row);
        db.BudgetItems.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return row.Id;
    }

    public async Task<bool> ChangeItemAsync(Guid periodId, Guid itemId, PlanItem item, CancellationToken cancellationToken)
    {
        BudgetItemRow? row = await db.BudgetItems.SingleOrDefaultAsync(i => i.Id == itemId && i.PeriodId == periodId, cancellationToken);
        if (row is null)
        {
            return false;
        }

        CopyTo(item, row);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveItemAsync(Guid periodId, Guid itemId, CancellationToken cancellationToken) =>
        await db.BudgetItems.Where(i => i.Id == itemId && i.PeriodId == periodId).ExecuteDeleteAsync(cancellationToken) > 0;

    public Task<bool> GroupNameTakenAsync(BudgetName name, Guid? exceptGroupId, CancellationToken cancellationToken) =>
        db.BudgetGroups.AnyAsync(g => g.Name == name.Value && g.Id != exceptGroupId, cancellationToken);

    public async Task<Guid> AddGroupAsync(Group group, CancellationToken cancellationToken)
    {
        BudgetGroupRow row = new() { Id = Guid.CreateVersion7(), Name = group.Name.Value, Position = group.Position };
        db.BudgetGroups.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return row.Id;
    }

    public async Task<bool> ChangeGroupAsync(Guid groupId, Group group, CancellationToken cancellationToken) =>
        await db.BudgetGroups
            .Where(g => g.Id == groupId)
            .ExecuteUpdateAsync(
                set => set.SetProperty(g => g.Name, group.Name.Value).SetProperty(g => g.Position, group.Position),
                cancellationToken) > 0;

    public Task<bool> GroupInUseAsync(Guid groupId, CancellationToken cancellationToken) =>
        db.BudgetItems.AnyAsync(i => i.GroupId == groupId, cancellationToken);

    public async Task<bool> DeleteGroupAsync(Guid groupId, CancellationToken cancellationToken) =>
        await db.BudgetGroups.Where(g => g.Id == groupId).ExecuteDeleteAsync(cancellationToken) > 0;

    public async Task SetCategoryGroupAsync(CategoryGroup assignment, CancellationToken cancellationToken)
    {
        string category = assignment.Category.Value;
        if (assignment.GroupId is not { } groupId)
        {
            await db.BudgetCategories.Where(c => c.Category == category).ExecuteDeleteAsync(cancellationToken);
            return;
        }

        await db.Database.ExecuteSqlAsync(
            $"""
            insert into expense.budget_categories (category, group_id) values ({category}, {groupId})
            on conflict (category) do update set group_id = excluded.group_id
            """,
            cancellationToken);
    }

    public async Task ReplaceTemplateAsync(Template template, CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        Dictionary<string, Guid> groups = await GroupsByNameAsync(template.Items.Select(item => item.Group.Value), cancellationToken);

        await db.BudgetFunds.Where(f => f.PeriodId == null).ExecuteDeleteAsync(cancellationToken);
        await db.BudgetItems.Where(i => i.PeriodId == null).ExecuteDeleteAsync(cancellationToken);

        db.BudgetFunds.AddRange(template.Funds.Select((fund, position) =>
        {
            BudgetFundRow row = new() { Id = Guid.CreateVersion7(), Position = position };
            CopyTo(fund, row);
            return row;
        }));
        db.BudgetItems.AddRange(template.Items.Select((item, position) =>
        {
            BudgetItemRow row = new() { Id = Guid.CreateVersion7(), Position = position };
            CopyTo(new PlanItem(groups[item.Group.Value], item.Name, item.Amount), row);
            return row;
        }));

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    /// <summary>Ids of the named groups; the ones that do not exist yet are added after the others.</summary>
    private async Task<Dictionary<string, Guid>> GroupsByNameAsync(IEnumerable<string> names, CancellationToken cancellationToken)
    {
        List<BudgetGroupRow> existing = await db.BudgetGroups.ToListAsync(cancellationToken);
        Dictionary<string, Guid> ids = existing.ToDictionary(group => group.Name, group => group.Id, StringComparer.Ordinal);
        int position = existing.Count == 0 ? 0 : existing.Max(group => group.Position) + 1;
        foreach (string name in names.Distinct(StringComparer.Ordinal).Where(name => !ids.ContainsKey(name)))
        {
            BudgetGroupRow row = new() { Id = Guid.CreateVersion7(), Name = name, Position = position++ };
            db.BudgetGroups.Add(row);
            ids[name] = row.Id;
        }

        return ids;
    }

    private static async Task<int> NextPositionAsync(IQueryable<int> positions, CancellationToken cancellationToken) =>
        (await positions.Select(position => (int?)position).MaxAsync(cancellationToken) ?? -1) + 1;

    private static BudgetPeriod ToDomain(BudgetPeriodRow row) =>
        BudgetPeriod.Create(row.Start, row.End).Match(
            period => period,
            _ => throw new InvalidDataException($"Budget period {row.Id} ends before it starts."));

    private static void CopyTo(Fund fund, BudgetFundRow row)
    {
        row.Name = fund.Name.Value;
        row.Amount = fund.Amount.Value;
    }

    private static void CopyTo(PlanItem item, BudgetItemRow row)
    {
        row.GroupId = item.GroupId;
        row.Name = item.Name.Value;
        row.Amount = item.Amount.Value;
    }
}
