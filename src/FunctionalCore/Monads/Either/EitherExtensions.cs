namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public static class EitherExtensions
{
  public static Result<R> ToResult<R>(this Either<Failure, R> either)
  {
    ArgumentNullException.ThrowIfNull(either);
    return either.Match(Fail.OfType<R>, Success.From);
  }
}
