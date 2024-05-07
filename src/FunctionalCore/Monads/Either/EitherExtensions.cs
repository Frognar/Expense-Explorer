namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public static class EitherExtensions
{
  public static Result<R> ToResult<R>(this Either<Failure, R> source)
  {
    ArgumentNullException.ThrowIfNull(source);
    return source.Match(Fail.OfType<R>, Success.From);
  }

  public static Either<L, R1> Select<L, R, R1>(this Either<L, R> source, Func<R, R1> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return source.MapRight(selector);
  }

  public static Either<L, R1> SelectMany<L, R, R1, R2>(
    this Either<L, R> source,
    Func<R, Either<L, R2>> selector,
    Func<R, R2, R1> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return source.FlatMapRight(right => selector(right).MapRight(t => projector(right, t)));
  }
}
