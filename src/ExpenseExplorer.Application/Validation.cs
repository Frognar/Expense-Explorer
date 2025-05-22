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

    public static Validated<Func<T2, TResult>> Apply<T1, T2, TResult>(
        this Func<T1, T2, TResult> map,
        Validated<T1> value) =>
        Succeed(map).Apply(value);

    public static Validated<Func<T2, TResult>> Apply<T1, T2, TResult>(
        this Validated<Func<T1, T2, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<Func<T2, TResult>>(errors.Concat(otherErrors)),
                value: _ => Failed<Func<T2, TResult>>(errors)),
            value: f => value.Match(
                errors: Failed<Func<T2, TResult>>,
                value: t1 => Succeed((T2 t2) => f(t1, t2))));

    public static Validated<TResult> Apply<T1, TResult>(
        this Validated<Func<T1, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<TResult>(errors.Concat(otherErrors)),
                value: _ => Failed<TResult>(errors)),
            value: f => value.Match(
                errors: Failed<TResult>,
                value: t1 => Succeed(f(t1))));

}