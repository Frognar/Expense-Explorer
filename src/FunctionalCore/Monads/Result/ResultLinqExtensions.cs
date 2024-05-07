namespace FunctionalCore.Monads;

#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks - Required to enable seamless LINQ integration with asynchronous operations
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods - Need to be named Select/SelectMany to work with LINQ query syntax
public static class ResultLinqExtensions
{
  public static Result<TResult> Select<T, TResult>(
    this Result<T> source,
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return source.Map(selector);
  }

  public static async Task<Result<TResult>> Select<T, TResult>(
    this Result<T> source,
    Func<T, Task<TResult>> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return await source.MapAsync(selector);
  }

  public static async Task<Result<TResult>> Select<T, TResult>(
    this Task<Result<T>> source,
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return await source.MapAsync(selector);
  }

  public static async Task<Result<TResult>> Select<T, TResult>(
    this Task<Result<T>> source,
    Func<T, Task<TResult>> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return await source.MapAsync(selector);
  }

  public static Result<TResult> SelectMany<T, U, TResult>(
    this Result<T> source,
    Func<T, Result<U>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return source.FlatMap(value => selector(value).Map(u => projector(value, u)));
  }

  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Result<T> source,
    Func<T, Task<Result<U>>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => (await selector(value)).Map(u => projector(value, u)));
  }

  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Result<T> source,
    Func<T, Result<U>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Result<T> source,
    Func<T, Task<Result<U>>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Task<Result<T>> source,
    Func<T, Result<U>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(value => selector(value).Map(u => projector(value, u)));
  }

  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Task<Result<T>> source,
    Func<T, Task<Result<U>>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => (await selector(value)).Map(u => projector(value, u)));
  }

  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Task<Result<T>> source,
    Func<T, Task<Result<U>>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => await (await selector(value)).MapAsync(u => projector(value, u)));
  }
}
#pragma warning restore VSTHRD200
#pragma warning restore VSTHRD003
