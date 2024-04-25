namespace FunctionalCore.Monads;

public static class Some
{
  public static Maybe<T> From<T>(T value) => Maybe<T>.Some(value);
}
