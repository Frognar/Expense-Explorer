namespace FunctionalCore.Monads;

public sealed class Either<L, R>
{
  private readonly IEither _either;

  private Either(IEither either)
  {
    _either = either;
  }

  private interface IEither
  {
    T Match<T>(Func<L, T> onLeft, Func<R, T> onRight);
  }

  public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
  {
    ArgumentNullException.ThrowIfNull(onLeft);
    ArgumentNullException.ThrowIfNull(onRight);
    return _either.Match(onLeft, onRight);
  }

  public Either<L1, R> MapLeft<L1>(Func<L, L1> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(left => Either<L1, R>.Left(map(left)), Either<L1, R>.Right);
  }

  public Either<L, R1> MapRight<R1>(Func<R, R1> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Either<L, R1>.Left, right => Either<L, R1>.Right(map(right)));
  }

  public Either<L, R1> Select<R1>(Func<R, R1> selector) => MapRight(selector);

  public Either<L1, R1> MapBoth<L1, R1>(Func<L, L1> lmap, Func<R, R1> rmap)
  {
    ArgumentNullException.ThrowIfNull(lmap);
    ArgumentNullException.ThrowIfNull(rmap);
    return Match(
      left => Either<L1, R1>.Left(lmap(left)),
      right => Either<L1, R1>.Right(rmap(right)));
  }

  public Either<L1, R> FlatMapLeft<L1>(Func<L, Either<L1, R>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(map, Either<L1, R>.Right);
  }

  public Either<L, R1> FlatMapRight<R1>(Func<R, Either<L, R1>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Either<L, R1>.Left, map);
  }

  public Either<L1, R1> FlatMapBoth<L1, R1>(Func<L, Either<L1, R1>> lmap, Func<R, Either<L1, R1>> rmap)
  {
    ArgumentNullException.ThrowIfNull(lmap);
    ArgumentNullException.ThrowIfNull(rmap);
    return Match(lmap, rmap);
  }

  public Either<L, R1> SelectMany<R1, T>(Func<R, Either<L, T>> selector, Func<R, T, R1> projector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return FlatMapRight(right => selector(right).MapRight(t => projector(right, t)));
  }

  internal static Either<L, R> Left(L left) => new(new EitherLeft(left));

  internal static Either<L, R> Right(R right) => new(new EitherRight(right));

  private sealed class EitherLeft : IEither
  {
    private readonly L _left;

    public EitherLeft(L left) => _left = left;

    public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight) => onLeft(_left);
  }

  private sealed class EitherRight : IEither
  {
    private readonly R _right;

    public EitherRight(R right) => _right = right;

    public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight) => onRight(_right);
  }
}
