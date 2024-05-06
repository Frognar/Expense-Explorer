namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public sealed class Result<T>
{
  private readonly IResult _resultType;

  private Result(IResult resultType) => _resultType = resultType;

  private interface IResult;

  public TResult Match<TResult>(Func<Failure, TResult> onFailure, Func<T, TResult> onSuccess)
  {
    ArgumentNullException.ThrowIfNull(onFailure);
    ArgumentNullException.ThrowIfNull(onSuccess);
    return _resultType switch
    {
      FailureType failureType => onFailure(failureType.Failure),
      SuccessType successType => onSuccess(successType.Value),
      _ => throw new InvalidOperationException("Unknown result type."),
    };
  }

  public Result<TResult> Map<TResult>(Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Result<TResult>.Fail, value => Result<TResult>.Success(map(value)));
  }

  public Result<TResult> FlatMap<TResult>(Func<T, Result<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Result<TResult>.Fail, map);
  }

  internal static Result<T> Success(T value) => new(new SuccessType(value));

  internal static Result<T> Fail(Failure failure) => new(new FailureType(failure));

  private readonly record struct FailureType(Failure Failure) : IResult;

  private readonly record struct SuccessType(T Value) : IResult;
}
