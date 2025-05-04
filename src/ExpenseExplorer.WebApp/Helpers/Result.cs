namespace ExpenseExplorer.WebApp.Helpers;

internal static class Result
{
    public static Result<T, TError> Success<T, TError>(T value) => Result<T, TError>.Success(value);
    public static Result<T, TError> Failure<T, TError>(TError error) => Result<T, TError>.Failure(error);
}

internal sealed class Result<T, TError>
{
    private readonly IResult _result;
    private Result(IResult result) => _result = result;

    public static Result<T, TError> Success(T value) => new(new SuccessResult(value));
    public static Result<T, TError> Failure(TError error) => new(new FailureResult(error));

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<TError, TResult> onError) =>
        _result switch
        {
            SuccessResult success => onSuccess(success.Value),
            FailureResult failure => onError(failure.Error),
            _ => throw new InvalidOperationException("Invalid result")
        };

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<TError, Task<TResult>> onError) =>
        _result switch
        {
            SuccessResult success => await onSuccess(success.Value),
            FailureResult failure => await onError(failure.Error),
            _ => throw new InvalidOperationException("Invalid result")
        };

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, TResult> onSuccess,
        Func<TError, Task<TResult>> onError) =>
        _result switch
        {
            SuccessResult success => onSuccess(success.Value),
            FailureResult failure => await onError(failure.Error),
            _ => throw new InvalidOperationException("Invalid result")
        };

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<TError, TResult> onError) =>
        _result switch
        {
            SuccessResult success => await onSuccess(success.Value),
            FailureResult failure => onError(failure.Error),
            _ => throw new InvalidOperationException("Invalid result")
        };

    private interface IResult;

    private sealed record SuccessResult(T Value) : IResult;

    private sealed record FailureResult(TError Error) : IResult;
}