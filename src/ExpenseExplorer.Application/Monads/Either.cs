namespace ExpenseExplorer.Application.Monads;

public class Either<L, R>
{
  private readonly L left;
  private readonly R right;
  private readonly bool isLeft;

  private Either(L left, R right, bool isLeft)
  {
    this.left = left;
    this.right = right;
    this.isLeft = isLeft;
  }

#pragma warning disable CA1000
  public static Either<L, R> Left(L left) => new(left, default!, true);

  public static Either<L, R> Right(R right) => new(default!, right, false);
#pragma warning restore CA1000

  public T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
  {
    ArgumentNullException.ThrowIfNull(onLeft);
    ArgumentNullException.ThrowIfNull(onRight);
    return isLeft ? onLeft(left) : onRight(right);
  }
}
