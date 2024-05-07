namespace FunctionalCore.Monads;

public sealed class Either<L, R>
{
  private readonly IEither _eitherType;

  private Either(IEither eitherType)
  {
    _eitherType = eitherType;
  }

  private interface IEither;

  public T Match<T>(Func<L, T> left, Func<R, T> right)
  {
    ArgumentNullException.ThrowIfNull(left);
    ArgumentNullException.ThrowIfNull(right);
    return _eitherType switch
    {
      EitherLeft leftType => left(leftType.Value),
      EitherRight rightType => right(rightType.Value),
      _ => throw new InvalidOperationException("Unknown either type."),
    };
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

  internal static Either<L, R> Left(L left) => new(new EitherLeft(left));

  internal static Either<L, R> Right(R right) => new(new EitherRight(right));

  private readonly record struct EitherLeft(L Value) : IEither;

  private readonly record struct EitherRight(R Value) : IEither;
}
