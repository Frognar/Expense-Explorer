using System.Collections.Immutable;

namespace ExpenseExplorer.Application;

public static class Option
{
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    public static Option<T> None<T>() => Option<T>.None();

    public static Option<TResult> Map<T, TResult>(
        this Option<T> source,
        Func<T, TResult> map) =>
        source.Match(
            none: None<TResult>,
            some: v => Some(map(v)));

    public static Task<Option<TResult>> MapAsync<T, TResult>(
        this Option<T> source,
        Func<T, Task<TResult>> map) =>
        source.MatchAsync(
            none: None<TResult>,
            some: async v => Some(await map(v)));

    public static Option<TResult> FlatMap<T, TResult>(
        this Option<T> source,
        Func<T, Option<TResult>> map) =>
    source.Match(
        none: None<TResult>,
        some: map);

    public static T OrElse<T>(
        this Option<T> source,
        Func<T> onNone) =>
        source.Match(
            none: onNone,
            some: v => v);

    public static Option<IEnumerable<T>> TraverseOption<T>(this IEnumerable<Option<T>> options) =>
        options.Aggregate(
            Some(Enumerable.Empty<T>()),
            (acc, opt) => acc.FlatMap(accList => opt.Map(accList.Append))
        );

    public static Option<ImmutableArray<T>> TraverseOptionToImmutableArray<T>(this IEnumerable<Option<T>> options) =>
        options.TraverseOption().Map(seq => seq.ToImmutableArray());
}

public sealed class Option<T>
{
    private readonly IOption _option;

    private Option(IOption option) => _option = option;

    internal static Option<T> Some(T value) => new(new SomeOption(value));
    internal static Option<T> None() => new(new NoneOption());

    public bool IsSome => Match(() => false, _ => true);
    public bool IsNone => Match(() => true, _ => false);

    public TResult Match<TResult>(
        Func<TResult> none,
        Func<T, TResult> some)
    {
        ArgumentNullException.ThrowIfNull(none);
        ArgumentNullException.ThrowIfNull(some);
        return _option switch
        {
            NoneOption => none(),
            SomeOption s => some(s.Value),
            _ => throw new InvalidOperationException("Invalid option")
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<TResult> none,
        Func<T, Task<TResult>> some)
    {
        ArgumentNullException.ThrowIfNull(none);
        ArgumentNullException.ThrowIfNull(some);
        return _option switch
        {
            NoneOption => none(),
            SomeOption s => await some(s.Value),
            _ => throw new InvalidOperationException("Invalid option")
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<Task<TResult>> none,
        Func<T, Task<TResult>> some)
    {
        ArgumentNullException.ThrowIfNull(none);
        ArgumentNullException.ThrowIfNull(some);
        return _option switch
        {
            NoneOption => await none(),
            SomeOption s => await some(s.Value),
            _ => throw new InvalidOperationException("Invalid option")
        };
    }

    private interface IOption;
    private sealed record SomeOption(T Value) : IOption;
    private sealed record NoneOption : IOption;
}