using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Budget;

/// <summary>The day of the month money usually comes or goes, e.g. the 10th for bills; for information only.</summary>
public sealed record DayOfMonth
{
    private DayOfMonth(int value) => Value = value;

    public int Value { get; }

    public static Result<DayOfMonth> Create(int value) =>
        value is >= 1 and <= 31
            ? Result.Success(new DayOfMonth(value))
            : Result.Failure<DayOfMonth>(new Error("DayOfMonth.OutOfRange", "The day must be between 1 and 31."));
}
