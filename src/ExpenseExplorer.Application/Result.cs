namespace ExpenseExplorer.Application;

public static class Result
{
    public static Result<T, TError> Success<T, TError>(T value) => Result<T, TError>.Success(value);
    public static Result<T, TError> Failure<T, TError>(TError error) => Result<T, TError>.Failure(error);
}

public sealed class Result<T, TError>
{
    private readonly IResult _result;
    private Result(IResult result) => _result = result;

    internal static Result<T, TError> Success(T value) => new(new SuccessResult(value));
    internal static Result<T, TError> Failure(TError error) => new(new FailureResult(error));

    public TResult Match<TResult>(
        Func<T, TResult> value,
        Func<TError, TResult> error)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(value);

        return _result switch
        {
            FailureResult failure => error(failure.Error),
            SuccessResult success => value(success.Value),
            _ => throw new InvalidOperationException("Invalid result")
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> value,
        Func<TError, Task<TResult>> error)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(value);

        return _result switch
        {
            FailureResult failure => await error(failure.Error),
            SuccessResult success => await value(success.Value),
            _ => throw new InvalidOperationException("Invalid result")
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, TResult> value,
        Func<TError, Task<TResult>> error)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(value);

        return _result switch
        {
            FailureResult failure => await error(failure.Error),
            SuccessResult success => value(success.Value),
            _ => throw new InvalidOperationException("Invalid result")
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<TError, TResult> error,
        Func<T, Task<TResult>> value)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(value);

        return _result switch
        {
            FailureResult failure => error(failure.Error),
            SuccessResult success => await value(success.Value),
            _ => throw new InvalidOperationException("Invalid result")
        };
    }

    private interface IResult;

    private sealed record SuccessResult(T Value) : IResult;

    private sealed record FailureResult(TError Error) : IResult;
}