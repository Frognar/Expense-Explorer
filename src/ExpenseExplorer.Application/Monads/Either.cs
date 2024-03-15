namespace ExpenseExplorer.Application.Monads;

public class Either<L, R>
{
  private Either(L left, R right, bool isLeft)
  {
  }

#pragma warning disable CA1000
  public static Either<L, R> Left(L left) => new(left, default!, true);
#pragma warning restore CA1000
}
