using ExpenseExplorer.Domain.Budget;

namespace ExpenseExplorer.Domain.Tests;

public class BudgetTests
{
    private static readonly BudgetPeriod October = BudgetPeriod.MonthFrom(new DateOnly(2026, 10, 5));

    private static readonly Guid Bills = Guid.NewGuid();
    private static readonly Guid Food = Guid.NewGuid();
    private static readonly Guid Fun = Guid.NewGuid();

    [Fact]
    public void A_month_runs_from_pay_day_to_the_day_before_the_next()
    {
        Assert.Equal((new DateOnly(2026, 10, 5), new DateOnly(2026, 11, 4)), (October.Start, October.End));
        Assert.Equal(31, October.Days);
    }

    [Theory]
    [InlineData("2026-10-01", 31)]
    [InlineData("2026-10-05", 31)]
    [InlineData("2026-11-04", 1)]
    [InlineData("2026-11-05", 0)]
    public void Days_left_count_today(string today, int daysLeft) =>
        Assert.Equal(daysLeft, October.DaysLeft(DateOnly.Parse(today, System.Globalization.CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2026-10-05", "2026-10-04", "BudgetPeriod.EndBeforeStart")]
    [InlineData("2026-10-05", "2026-12-06", "BudgetPeriod.TooLong")]
    public void Period_must_be_ordered_and_not_too_long(string start, string end, string code)
    {
        var result = BudgetPeriod.Create(
            DateOnly.Parse(start, System.Globalization.CultureInfo.InvariantCulture),
            DateOnly.Parse(end, System.Globalization.CultureInfo.InvariantCulture));

        Assert.Equal(code, Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void Free_pool_is_what_groups_and_unplanned_spending_leave_of_the_funds()
    {
        BudgetSummary summary = BudgetSummary.Calculate(
            October,
            new DateOnly(2026, 10, 25),
            funds: 10_000m,
            [
                new GroupPlan(Bills, "Rachunki", 4_000m, ["Media", "Kredyt"]),
                new GroupPlan(Food, "Jedzenie", 3_000m, ["Spożywcze"]),
                new GroupPlan(Fun, "Rozrywka", 0m, ["Kino"]),
            ],
            new Dictionary<string, decimal>
            {
                ["Media"] = 1_000m,
                ["Kredyt"] = 3_059m,
                ["Spożywcze"] = 1_200m,
                ["Kino"] = 80m,
                ["Prezenty"] = 150m,
            });

        Assert.Equal(
            [("Rachunki", 4_000m, 4_059m, -59m), ("Jedzenie", 3_000m, 1_200m, 1_800m), ("Rozrywka", 0m, 80m, -80m)],
            summary.Groups.Select(group => (group.Name, group.Planned, group.Spent, group.Remaining)));
        Assert.Equal(150m, Assert.Single(summary.Unassigned).Value);
        Assert.Equal(7_000m, summary.Planned);
        Assert.Equal(5_489m, summary.Spent);

        // 10 000 − 4 059 (over plan) − 3 000 (plan still open) − 80 (no plan) − 150 (no group)
        Assert.Equal(2_711m, summary.FreePool);
        Assert.Equal(11, summary.DaysLeft);
        Assert.Equal(246.45m, summary.PerDay);
    }

    [Fact]
    public void After_the_period_there_is_nothing_per_day()
    {
        BudgetSummary summary = BudgetSummary.Calculate(October, new DateOnly(2026, 11, 10), 100m, [], new Dictionary<string, decimal>());

        Assert.Equal(0, summary.DaysLeft);
        Assert.Null(summary.PerDay);
    }

    [Theory]
    [InlineData(0, "FundAmount.Zero")]
    [InlineData(0.001, "Money.TooPrecise")]
    public void Fund_amount_is_not_zero_and_in_grosze(decimal amount, string code) =>
        Assert.Equal(code, Assert.Single(FundAmount.Create(amount).Errors).Code);

    [Fact]
    public void Fund_amount_may_be_negative() =>
        Assert.Equal(-3000m, Given.Valid(FundAmount.Create(-3000m)).Value);

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void Day_of_month_is_between_1_and_31(int day) =>
        Assert.Equal("DayOfMonth.OutOfRange", Assert.Single(DayOfMonth.Create(day).Errors).Code);
}
