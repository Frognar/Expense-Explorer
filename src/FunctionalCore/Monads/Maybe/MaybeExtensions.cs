namespace FunctionalCore.Monads;

using FunctionalCore.Failures;

#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks - Required to enable seamless LINQ integration with asynchronous operations
public static class MaybeExtensions
{
  public static Result<T> ToResult<T>(this Maybe<T> source, Func<Failure> onNoneFailureFactory)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(onNoneFailureFactory);
    return source.Match(() => Fail.OfType<T>(onNoneFailureFactory()), Success.From);
  }

  public static T OrElse<T>(this Maybe<T> source, Func<T> fallback)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(fallback);
    return source.Match(fallback, value => value);
  }

  public static async Task<Maybe<TResult>> MapAsync<T, TResult>(this Task<Maybe<T>> source, Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return (await source).Map(map);
  }

  public static async Task<Maybe<TResult>> MapAsync<T, TResult>(this Task<Maybe<T>> source, Func<T, Task<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await (await source).MapAsync(map);
  }

  public static async Task<Maybe<TResult>> FlatMapAsync<T, TResult>(
    this Task<Maybe<T>> source,
    Func<T, Maybe<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return (await source).FlatMap(map);
  }

  public static async Task<Maybe<TResult>> FlatMapAsync<T, TResult>(
    this Task<Maybe<T>> source,
    Func<T, Task<Maybe<TResult>>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await (await source).FlatMapAsync(map);
  }
}
#pragma warning restore VSTHRD003
