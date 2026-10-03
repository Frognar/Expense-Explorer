using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Budget;

/// <summary>The days a budget covers, usually from one pay day to the day before the next.</summary>
public sealed record BudgetPeriod
{
    /// <summary>Long enough for any month plus a few days of a late pay day.</summary>
    public const int MaxDays = 62;

    private BudgetPeriod(DateOnly start, DateOnly end) => (Start, End) = (start, end);

    public DateOnly Start { get; }

    public DateOnly End { get; }

    public int Days => End.DayNumber - Start.DayNumber + 1;

    public static Result<BudgetPeriod> Create(DateOnly start, DateOnly end) =>
        (end.DayNumber - start.DayNumber + 1) switch
        {
            < 1 => Result.Failure<BudgetPeriod>(new Error("BudgetPeriod.EndBeforeStart", "The period ends before it starts.")),
            > MaxDays => Result.Failure<BudgetPeriod>(new Error("BudgetPeriod.TooLong", $"A period can last at most {MaxDays} days.")),
            _ => Result.Success(new BudgetPeriod(start, end)),
        };

    /// <summary>From <paramref name="start"/> to the day before the same day a month later.</summary>
    public static BudgetPeriod MonthFrom(DateOnly start) => new(start, start.AddMonths(1).AddDays(-1));

    public bool Contains(DateOnly day) => day >= Start && day <= End;

    public bool Overlaps(BudgetPeriod other) => Start <= other.End && other.Start <= End;

    /// <summary>Days still to spend, today included: all of them before the period, none after it.</summary>
    public int DaysLeft(DateOnly today) =>
        Math.Clamp(End.DayNumber - today.DayNumber + 1, 0, Days);
}
