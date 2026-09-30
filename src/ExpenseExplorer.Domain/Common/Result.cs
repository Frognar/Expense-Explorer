using System.Collections.Immutable;

namespace ExpenseExplorer.Domain.Common;

/// <summary>
/// Outcome of an operation: either a value or at least one error, never both.
/// </summary>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T value)
    {
        _value = value;
        Errors = [];
    }

    private Result(ImmutableArray<Error> errors)
    {
        if (errors.IsDefaultOrEmpty)
        {
            throw new ArgumentException("A failure needs at least one error.", nameof(errors));
        }

        Errors = errors;
    }

    public bool IsSuccess => Errors.IsEmpty;

    public ImmutableArray<Error> Errors { get; }

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<ImmutableArray<Error>, TOut> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(Errors);

    public Result<TOut> Map<TOut>(Func<T, TOut> map) =>
        Match(value => Result.Success(map(value)), Result.Failure<TOut>);

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind) =>
        Match(bind, Result.Failure<TOut>);

    public Result<T> ForTarget(string target) =>
        Match(
            _ => this,
            errors => Result.Failure<T>([.. errors.Select(error => error.For(target))]));

    internal static Result<T> FromValue(T value) => new(value);

    internal static Result<T> FromErrors(ImmutableArray<Error> errors) => new(errors);
}

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.FromValue(value);

    public static Result<T> Failure<T>(Error error) => Result<T>.FromErrors([error]);

    public static Result<T> Failure<T>(ImmutableArray<Error> errors) => Result<T>.FromErrors(errors);
}
