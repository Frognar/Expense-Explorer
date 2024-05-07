namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public sealed class Result<T>
{
  private readonly IResult _resultType;

  private Result(IResult resultType) => _resultType = resultType;

  private interface IResult;

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
