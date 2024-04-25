namespace FunctionalCore.Monads;

public static class Success
{
  public static Result<T> From<T>(T value) => Result<T>.Success(value);
}
