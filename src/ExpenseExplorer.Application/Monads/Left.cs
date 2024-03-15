namespace ExpenseExplorer.Application.Monads;

public static class Left
{
  public static Either<L, R> From<L, R>(L left) => Either<L, R>.Left(left);
}
