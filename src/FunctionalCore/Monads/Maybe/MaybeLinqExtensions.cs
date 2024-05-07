namespace FunctionalCore.Monads;

#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks - Required to enable seamless LINQ integration with asynchronous operations
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods - Need to be named Select/SelectMany to work with LINQ query syntax
public static class MaybeLinqExtensions
{
  public static Maybe<TResult> Select<T, TResult>(
    this Maybe<T> source,
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return source.Map(selector);
  }

  public static async Task<Maybe<TResult>> Select<T, TResult>(
    this Maybe<T> source,
    Func<T, Task<TResult>> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return await source.MapAsync(selector);
  }

  public static async Task<Maybe<TResult>> Select<T, TResult>(
    this Task<Maybe<T>> source,
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return await source.MapAsync(selector);
  }

  public static async Task<Maybe<TResult>> Select<T, TResult>(
    this Task<Maybe<T>> source,
    Func<T, Task<TResult>> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return await source.MapAsync(selector);
  }

  public static Maybe<TResult> SelectMany<T, TResult, T1>(
    this Maybe<T> source,
    Func<T, Maybe<T1>> selector,
    Func<T, T1, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return source.FlatMap(v => selector(v).Map(i => projector(v, i)));
  }

  public static async Task<Maybe<TResult>> SelectMany<T, U, TResult>(
    this Maybe<T> source,
    Func<T, Task<Maybe<U>>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => (await selector(value)).Map(u => projector(value, u)));
  }

  public static async Task<Maybe<TResult>> SelectMany<T, U, TResult>(
    this Maybe<T> source,
    Func<T, Maybe<U>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public static async Task<Maybe<TResult>> SelectMany<T, U, TResult>(
    this Maybe<T> source,
    Func<T, Task<Maybe<U>>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await source.FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public static async Task<Maybe<TResult>> SelectMany<T, TResult, T1>(
    this Task<Maybe<T>> source,
    Func<T, Maybe<T1>> selector,
    Func<T, T1, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return (await source).FlatMap(v => selector(v).Map(i => projector(v, i)));
  }

  public static async Task<Maybe<TResult>> SelectMany<T, U, TResult>(
    this Task<Maybe<T>> source,
    Func<T, Task<Maybe<U>>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await (await source).FlatMapAsync(async value => (await selector(value)).Map(u => projector(value, u)));
  }

  public static async Task<Maybe<TResult>> SelectMany<T, U, TResult>(
    this Task<Maybe<T>> source,
    Func<T, Maybe<U>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await (await source).FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public static async Task<Maybe<TResult>> SelectMany<T, U, TResult>(
    this Task<Maybe<T>> source,
    Func<T, Task<Maybe<U>>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await (await source).FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }
}
#pragma warning restore VSTHRD200
#pragma warning restore VSTHRD003
