namespace FunctionalCore;

public static class Extensions
{
  public static T Apply<T>(this T value, Func<T, T> transformation)
  {
    ArgumentNullException.ThrowIfNull(transformation);
    return transformation(value);
  }
}
