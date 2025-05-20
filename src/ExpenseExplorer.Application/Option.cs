namespace ExpenseExplorer.Application;

public static class Option
{
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    public static Option<T> None<T>() => Option<T>.None();
}

public sealed class Option<T>
{
    private readonly IOption _option;

    private Option(IOption option) => _option = option;

    internal static Option<T> Some(T value) => new(new SomeOption(value));
    internal static Option<T> None() => new(new NoneOption());

    public TResult Match<TResult>(
        Func<TResult> none,
        Func<T, TResult> some
        )
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

    private interface IOption;
    private sealed record SomeOption(T Value) : IOption;
    private sealed record NoneOption : IOption;
}