namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public sealed class Result<T>
{
  private readonly Either<Failure, T> _either;

  private Result(Either<Failure, T> either) => _either = either;

  public TResult Match<TResult>(Func<Failure, TResult> onFailure, Func<T, TResult> onSuccess)
  {
    ArgumentNullException.ThrowIfNull(onFailure);
    ArgumentNullException.ThrowIfNull(onSuccess);
    return _either.Match(onFailure, onSuccess);
  }

  public Result<TResult> Map<TResult>(Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Result<TResult>.Fail, value => Result<TResult>.Success(map(value)));
  }

  public Result<TResult> Select<TResult>(Func<T, TResult> selector)
  {
    return Map(selector);
  }

  public Result<TResult> FlatMap<TResult>(Func<T, Result<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Result<TResult>.Fail, map);
  }

  public Result<TResult> SelectMany<U, TResult>(Func<T, Result<U>> selector, Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return FlatMap(value => selector(value).Map(u => projector(value, u)));
  }

  internal static Result<T> Success(T value) => new(Right.From<Failure, T>(value));

  internal static Result<T> Fail(Failure failure) => new(Left.From<Failure, T>(failure));
}
