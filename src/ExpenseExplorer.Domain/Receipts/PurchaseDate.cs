using System.Globalization;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record PurchaseDate
{
    private PurchaseDate(DateOnly value) => Value = value;

    public DateOnly Value { get; }

    public static Result<PurchaseDate> Create(DateOnly value, DateOnly today) =>
        value > today
            ? Result.Failure<PurchaseDate>(new Error("PurchaseDate.InFuture", "Purchase date cannot be in the future."))
            : Result.Success(new PurchaseDate(value));

    public override string ToString() => Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
