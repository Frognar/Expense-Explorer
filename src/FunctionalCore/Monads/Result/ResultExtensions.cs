namespace FunctionalCore.Monads;

#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks - Required to enable seamless LINQ integration with asynchronous operations
public static class ResultExtensions
{
  public static async Task<Result<TResult>> MapAsync<T, TResult>(this Result<T> source, Func<T, Task<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await source.Match(
      failure => Task.FromResult(Fail.OfType<TResult>(failure)),
      async value => Success.From(await map(value)));
  }

  public static async Task<Result<TResult>> MapAsync<T, TResult>(
    this Task<Result<T>> source,
    Func<T, Task<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await (await source).Match(
      failure => Task.FromResult(Fail.OfType<TResult>(failure)),
      async value => Success.From(await map(value)));
  }

  public static async Task<Result<TResult>> MapAsync<T, TResult>(
    this Task<Result<T>> source,
    Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return (await source).Map(map);
  }

  public static async Task<Result<TResult>> FlatMapAsync<T, TResult>(
    this Result<T> source,
    Func<T, Task<Result<TResult>>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await source.Match(
      failure => Task.FromResult(Fail.OfType<TResult>(failure)),
      async value => await map(value));
  }

  public static async Task<Result<TResult>> FlatMapAsync<T, TResult>(
    this Task<Result<T>> source,
    Func<T, Task<Result<TResult>>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await (await source).Match(
      failure => Task.FromResult(Fail.OfType<TResult>(failure)),
      async value => await map(value));
  }

  public static async Task<Result<TResult>> FlatMapAsync<T, TResult>(
    this Task<Result<T>> source,
    Func<T, Result<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return (await source).FlatMap(map);
  }
}
#pragma warning restore VSTHRD003
