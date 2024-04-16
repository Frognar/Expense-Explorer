namespace FunctionalCore.Monads;

public static class None
{
  public static Maybe<T> OfType<T>() => Maybe<T>.None();
}
