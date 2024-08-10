namespace FunctionalCore;

using DotMaybe;
using DotResult;

public static class MaybeExtensions
{
  public static Result<T> ToResult<T>(this Maybe<T> maybe, Func<Failure> failureFactory)
  {
    return maybe.Match(() => Fail.OfType<T>(failureFactory()), Success.From);
  }
}