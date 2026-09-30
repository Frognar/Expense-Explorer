using System.Collections.Immutable;

namespace ExpenseExplorer.Domain.Common;

/// <summary>
/// Combines independent results. Unlike <see cref="Result{T}.Bind{TOut}"/> it collects
/// the errors of every failed input instead of stopping at the first one.
/// </summary>
public static class ResultCombine
{
    public static Result<TOut> Combine<T1, T2, TOut>(
        Result<T1> r1,
        Result<T2> r2,
        Func<T1, T2, TOut> combine) =>
        Collect(r1.Errors, r2.Errors) is { IsEmpty: false } errors
            ? Result.Failure<TOut>(errors)
            : Result.Success(combine(ValueOf(r1), ValueOf(r2)));

    public static Result<TOut> Combine<T1, T2, T3, TOut>(
        Result<T1> r1,
        Result<T2> r2,
        Result<T3> r3,
        Func<T1, T2, T3, TOut> combine) =>
        Collect(r1.Errors, r2.Errors, r3.Errors) is { IsEmpty: false } errors
            ? Result.Failure<TOut>(errors)
            : Result.Success(combine(ValueOf(r1), ValueOf(r2), ValueOf(r3)));

    public static Result<TOut> Combine<T1, T2, T3, T4, TOut>(
        Result<T1> r1,
        Result<T2> r2,
        Result<T3> r3,
        Result<T4> r4,
        Func<T1, T2, T3, T4, TOut> combine) =>
        Collect(r1.Errors, r2.Errors, r3.Errors, r4.Errors) is { IsEmpty: false } errors
            ? Result.Failure<TOut>(errors)
            : Result.Success(combine(ValueOf(r1), ValueOf(r2), ValueOf(r3), ValueOf(r4)));

    public static Result<TOut> Combine<T1, T2, T3, T4, T5, TOut>(
        Result<T1> r1,
        Result<T2> r2,
        Result<T3> r3,
        Result<T4> r4,
        Result<T5> r5,
        Func<T1, T2, T3, T4, T5, TOut> combine) =>
        Collect(r1.Errors, r2.Errors, r3.Errors, r4.Errors, r5.Errors) is { IsEmpty: false } errors
            ? Result.Failure<TOut>(errors)
            : Result.Success(combine(ValueOf(r1), ValueOf(r2), ValueOf(r3), ValueOf(r4), ValueOf(r5)));

    /// <summary>Turns many results into one: all values in order, or every error.</summary>
    public static Result<IReadOnlyList<T>> Sequence<T>(IEnumerable<Result<T>> results)
    {
        List<Result<T>> all = [.. results];
        ImmutableArray<Error> errors = [.. all.SelectMany(result => result.Errors)];
        return errors.IsEmpty
            ? Result.Success<IReadOnlyList<T>>([.. all.Select(ValueOf)])
            : Result.Failure<IReadOnlyList<T>>(errors);
    }

    private static ImmutableArray<Error> Collect(params ReadOnlySpan<ImmutableArray<Error>> errors)
    {
        ImmutableArray<Error>.Builder all = ImmutableArray.CreateBuilder<Error>();
        foreach (ImmutableArray<Error> error in errors)
        {
            all.AddRange(error);
        }

        return all.ToImmutable();
    }

    private static T ValueOf<T>(Result<T> result) =>
        result.Match(value => value, _ => throw new InvalidOperationException("Result is not a success."));
}
