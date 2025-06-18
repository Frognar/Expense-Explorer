namespace ExpenseExplorer.Application;

public static class Validation
{
    public static Validated<T> Succeed<T>(T value) => Validated<T>.Succeed(value);
    public static Validated<T> Failed<T>(IEnumerable<ValidationError> errors) => Validated<T>.Failed(errors);

    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> map,
        Validated<T1> value) =>
        Succeed(map).Apply(value);

    public static Validated<Func<T2, T3, T4, T5, T6, T7, T8, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<Func<T2, T3, T4, T5, T6, T7, T8, TResult>>(errors.Concat(otherErrors)),
                value: _ => Failed<Func<T2, T3, T4, T5, T6, T7, T8, TResult>>(errors)),
            value: f => value.Match(
                errors: Failed<Func<T2, T3, T4, T5, T6, T7, T8, TResult>>,
                value: t1 => Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => f(t1, t2, t3, t4, t5, t6, t7, t8))));

    public static Validated<Func<T2, T3, T4, T5, T6, T7, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, T7, TResult> map,
        Validated<T1> value) =>
        Succeed(map).Apply(value);

    public static Validated<Func<T2, T3, T4, T5, T6, T7, TResult>> Apply<T1, T2, T3, T4, T5, T6, T7, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<Func<T2, T3, T4, T5, T6, T7, TResult>>(errors.Concat(otherErrors)),
                value: _ => Failed<Func<T2, T3, T4, T5, T6, T7, TResult>>(errors)),
            value: f => value.Match(
                errors: Failed<Func<T2, T3, T4, T5, T6, T7, TResult>>,
                value: t1 => Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => f(t1, t2, t3, t4, t5, t6, t7))));

    public static Validated<Func<T2, T3, T4, T5, T6, TResult>> Apply<T1, T2, T3, T4, T5, T6, TResult>(
        this Func<T1, T2, T3, T4, T5, T6, TResult> map,
        Validated<T1> value) =>
        Succeed(map).Apply(value);

    public static Validated<Func<T2, T3, T4, T5, T6, TResult>> Apply<T1, T2, T3, T4, T5, T6, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, T6, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<Func<T2, T3, T4, T5, T6, TResult>>(errors.Concat(otherErrors)),
                value: _ => Failed<Func<T2, T3, T4, T5, T6, TResult>>(errors)),
            value: f => value.Match(
                errors: Failed<Func<T2, T3, T4, T5, T6, TResult>>,
                value: t1 => Succeed((T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => f(t1, t2, t3, t4, t5, t6))));

    public static Validated<Func<T2, T3, T4, T5, TResult>> Apply<T1, T2, T3, T4, T5, TResult>(
        this Func<T1, T2, T3, T4, T5, TResult> map,
        Validated<T1> value) =>
        Succeed(map).Apply(value);

    public static Validated<Func<T2, T3, T4, T5, TResult>> Apply<T1, T2, T3, T4, T5, TResult>(
        this Validated<Func<T1, T2, T3, T4, T5, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<Func<T2, T3, T4, T5, TResult>>(errors.Concat(otherErrors)),
                value: _ => Failed<Func<T2, T3, T4, T5, TResult>>(errors)),
            value: f => value.Match(
                errors: Failed<Func<T2, T3, T4, T5, TResult>>,
                value: t1 => Succeed((T2 t2, T3 t3, T4 t4, T5 t5) => f(t1, t2, t3, t4, t5))));

    public static Validated<Func<T2, T3, T4, TResult>> Apply<T1, T2, T3, T4, TResult>(
        this Func<T1, T2, T3, T4, TResult> map,
        Validated<T1> value) =>
        Succeed(map).Apply(value);

    public static Validated<Func<T2, T3, T4, TResult>> Apply<T1, T2, T3, T4, TResult>(
        this Validated<Func<T1, T2, T3, T4, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<Func<T2, T3, T4, TResult>>(errors.Concat(otherErrors)),
                value: _ => Failed<Func<T2, T3, T4, TResult>>(errors)),
            value: f => value.Match(
                errors: Failed<Func<T2, T3, T4, TResult>>,
                value: t1 => Succeed((T2 t2, T3 t3, T4 t4) => f(t1, t2, t3, t4))));

    public static Validated<Func<T2, T3, TResult>> Apply<T1, T2, T3, TResult>(
        this Func<T1, T2, T3, TResult> map,
        Validated<T1> value) =>
        Succeed(map).Apply(value);

    public static Validated<Func<T2, T3, TResult>> Apply<T1, T2, T3, TResult>(
        this Validated<Func<T1, T2, T3, TResult>> map,
        Validated<T1> value) =>
        map.Match(
            errors: errors => value.Match(
                errors: otherErrors => Failed<Func<T2, T3, TResult>>(errors.Concat(otherErrors)),
                value: _ => Failed<Func<T2, T3, TResult>>(errors)),
            value: f => value.Match(
                errors: Failed<Func<T2, T3, TResult>>,
                value: t1 => Succeed((T2 t2, T3 t3) => f(t1, t2, t3))));

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