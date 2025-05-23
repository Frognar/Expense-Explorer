namespace ExpenseExplorer.Application;

public static class Result
{
    public static Result<T, TError> Success<T, TError>(T value) => Result<T, TError>.Success(value);
    public static Result<T, TError> Failure<T, TError>(TError error) => Result<T, TError>.Failure(error);

    public static Result<T, TErrorResult> MapError<T, TError, TErrorResult>(
        this Result<T, TError> source,
        Func<TError, TErrorResult> map)
    {
        return source.Match(
            error: e => Failure<T, TErrorResult>(map(e)),
            value: Success<T, TErrorResult>);
    }

    public static Result<TResult, TError> Map<T, TResult, TError>(
        this Result<T, TError> source,
        Func<T, TResult> map)
    {
        return source.Match(
            error: Failure<TResult, TError>,
            value: v => Success<TResult, TError>(map(v)));
    }

    public static Task<Result<TResult, TError>> MapAsync<T, TResult, TError>(
        this Result<T, TError> source,
        Func<T, Task<TResult>> map)
    {
        return source.MatchAsync(
            error: Failure<TResult, TError>,
            value: async v => Success<TResult, TError>(await map(v)));
    }
}

public sealed class Result<T, TError>
{
    private readonly IResult _result;
    private Result(IResult result) => _result = result;

    internal static Result<T, TError> Success(T value) => new(new SuccessResult(value));
    internal static Result<T, TError> Failure(TError error) => new(new FailureResult(error));

    public TResult Match<TResult>(
        Func<TError, TResult> error,
        Func<T, TResult> value)
    {
        return _result switch
        {
            FailureResult failure => error(failure.Error),
            SuccessResult success => value(success.Value),
            _ => throw new InvalidOperationException("Invalid result")
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<TError, Task<TResult>> error,
        Func<T, Task<TResult>> value)
    {
        return _result switch
        {
            FailureResult failure => await error(failure.Error),
            SuccessResult success => await value(success.Value),
            _ => throw new InvalidOperationException("Invalid result")
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<TError, Task<TResult>> error,
        Func<T, TResult> value)
    {
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
        return _result switch
        {
            FailureResult failure => error(failure.Error),
            SuccessResult success => await value(success.Value),
            _ => throw new InvalidOperationException("Invalid result")
        };
    }

    private interface IResult;

    private sealed record FailureResult(TError Error) : IResult;

    private sealed record SuccessResult(T Value) : IResult;
}