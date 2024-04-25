namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

public static class Fail
{
  public static Result<T> OfType<T>(Failure failure) => Result<T>.Fail(failure);
}
