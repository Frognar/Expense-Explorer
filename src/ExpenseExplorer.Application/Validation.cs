namespace ExpenseExplorer.Application;

public static class Validation
{
    public static Validated<T> Succeed<T>(T value) => Validated<T>.Succeed(value);
    public static Validated<T> Failed<T>(IEnumerable<ValidationError> errors) => Validated<T>.Failed(errors);

    public static Validated<TResult> Join<T, T1, TResult>(
        this Validated<T> source,
        Validated<T1> other,
        Func<T, T1, TResult> join)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Match(
            errors: sourceErrors =>
                other.Match(
                    errors: otherErrors => Failed<TResult>(sourceErrors.Concat(otherErrors)),
                    value: _ => Failed<TResult>(sourceErrors)),
            value: sourceValue =>
                other.Match(
                    errors: Failed<TResult>,
                    value: otherValue => Succeed(join(sourceValue, otherValue))));
    }
}