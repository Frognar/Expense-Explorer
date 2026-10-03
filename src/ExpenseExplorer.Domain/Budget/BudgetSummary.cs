namespace ExpenseExplorer.Domain.Budget;

/// <summary>A budget group with what is planned for it and the receipt categories it collects.</summary>
public sealed record GroupPlan(Guid GroupId, string Name, decimal Planned, IReadOnlyCollection<string> Categories);

public sealed record GroupResult(Guid GroupId, string Name, decimal Planned, decimal Spent)
{
    public decimal Remaining => Planned - Spent;

    /// <summary>What the group takes from the period: its plan, or more once spending goes over it.</summary>
    public decimal Taken => Math.Max(Planned, Spent);
}

/// <summary>
/// The state of one period. Spending comes only from receipts. A group takes its plan or what was spent,
/// whichever is more; categories in no group and overspending come out of the free pool.
/// </summary>
public sealed record BudgetSummary(
    decimal Funds,
    IReadOnlyList<GroupResult> Groups,
    IReadOnlyDictionary<string, decimal> Unassigned,
    int DaysLeft)
{
    public decimal Planned => Groups.Sum(group => group.Planned);

    public decimal Spent => Groups.Sum(group => group.Spent) + Unassigned.Values.Sum();

    /// <summary>Roughly what will be left at the end of the period.</summary>
    public decimal FreePool => Funds - Groups.Sum(group => group.Taken) - Unassigned.Values.Sum();

    /// <summary>The free pool spread over the days left; none once the period is over.</summary>
    public decimal? PerDay => DaysLeft > 0 ? decimal.Round(FreePool / DaysLeft, 2, MidpointRounding.ToZero) : null;

    public static BudgetSummary Calculate(
        BudgetPeriod period,
        DateOnly today,
        decimal funds,
        IReadOnlyList<GroupPlan> groups,
        IReadOnlyDictionary<string, decimal> spentByCategory)
    {
        HashSet<string> assigned = [.. groups.SelectMany(group => group.Categories)];
        return new BudgetSummary(
            funds,
            [.. groups.Select(group => new GroupResult(
                group.GroupId,
                group.Name,
                group.Planned,
                group.Categories.Sum(category => spentByCategory.GetValueOrDefault(category))))],
            spentByCategory
                .Where(spent => !assigned.Contains(spent.Key))
                .ToDictionary(spent => spent.Key, spent => spent.Value),
            period.DaysLeft(today));
    }
}
