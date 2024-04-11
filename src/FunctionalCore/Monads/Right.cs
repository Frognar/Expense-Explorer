namespace FunctionalCore.Monads;

public static class Right
{
  public static Either<L, R> From<L, R>(R right) => Either<L, R>.Right(right);
}
