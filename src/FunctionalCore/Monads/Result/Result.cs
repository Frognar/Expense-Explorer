namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public readonly partial record struct Result<T>
{
  private readonly IResult _resultType;

  private Result(IResult resultType) => _resultType = resultType;

  public TResult Match<TResult>(Func<Failure, TResult> failure, Func<T, TResult> success)
  {
    ArgumentNullException.ThrowIfNull(failure);
    ArgumentNullException.ThrowIfNull(success);
    return _resultType switch
    {
      FailureType failureType => failure(failureType.Failure),
      SuccessType successType => success(successType.Value),
      _ => throw new InvalidOperationException("Unknown result type."),
    };
  }

  public async Task<TResult> MatchAsync<TResult>(Func<Failure, Task<TResult>> failure, Func<T, Task<TResult>> success)
  {
    ArgumentNullException.ThrowIfNull(failure);
    ArgumentNullException.ThrowIfNull(success);
    return _resultType switch
    {
      FailureType failureType => await failure(failureType.Failure).ConfigureAwait(false),
      SuccessType successType => await success(successType.Value).ConfigureAwait(false),
      _ => throw new InvalidOperationException("Unknown result type."),
    };
  }

  public Result<TResult> Map<TResult>(Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Result<TResult>.Fail, value => Result<TResult>.Success(map(value)));
  }

  public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return await MatchAsync(
      failure => Task.FromResult(Result<TResult>.Fail(failure)),
      async value => Result<TResult>.Success(await map(value)));
  }

  public Result<TResult> FlatMap<TResult>(Func<T, Result<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Result<TResult>.Fail, map);
  }

  public async Task<Result<TResult>> FlatMapAsync<TResult>(Func<T, Task<Result<TResult>>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return await MatchAsync(failure => Task.FromResult(Result<TResult>.Fail(failure)), map);
  }

  internal static Result<T> Success(T value) => new(new SuccessType(value));

  internal static Result<T> Fail(Failure failure) => new(new FailureType(failure));
}
