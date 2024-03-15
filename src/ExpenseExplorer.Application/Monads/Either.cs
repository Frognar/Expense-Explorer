namespace ExpenseExplorer.Application.Monads;

public sealed class Either<L, R>
{
  private readonly IEither either;

  private Either(IEither either)
  {
    this.either = either;
  }

  private interface IEither
  {
    T Match<T>(Func<L, T> onLeft, Func<R, T> onRight);
  }

#pragma warning disable CA1000
  public static Either<L, R> Left(L left) => new(new EitherLeft(left));

  public static Either<L, R> Right(R right) => new(new EitherRight(right));
#pragma warning restore CA1000

  public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
  {
    ArgumentNullException.ThrowIfNull(onLeft);
    ArgumentNullException.ThrowIfNull(onRight);
    return either.Match(onLeft, onRight);
  }

  private sealed class EitherLeft : IEither
  {
    private readonly L left;

    public EitherLeft(L left) => this.left = left;

    public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight) => onLeft(left);
  }

  private sealed class EitherRight : IEither
  {
    private readonly R right;

    public EitherRight(R right) => this.right = right;

    public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight) => onRight(right);
  }
}
