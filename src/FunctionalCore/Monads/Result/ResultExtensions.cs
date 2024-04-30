namespace FunctionalCore.Monads;

using System.Diagnostics.CodeAnalysis;

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

  [SuppressMessage(
    "Design",
    "VSTHRD200:Use \"Async\" suffix for async methods",
    Justification = "Need to be named Select to work with LINQ query syntax")]
  [SuppressMessage(
    "Design",
    "VSTHRD003:Avoid awaiting foreign Tasks",
    Justification = "Required to enable seamless LINQ integration with asynchronous operations.")]
  public static async Task<Result<TResult>> Select<T, TResult>(
    this Task<Result<T>> source,
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return (await source).Map(selector);
  }

  [SuppressMessage(
    "Design",
    "VSTHRD200:Use \"Async\" suffix for async methods",
    Justification = "Need to be named SelectMany to work with LINQ query syntax")]
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

  [SuppressMessage(
    "Design",
    "VSTHRD200:Use \"Async\" suffix for async methods",
    Justification = "Need to be named SelectMany to work with LINQ query syntax")]
  [SuppressMessage(
    "Design",
    "VSTHRD003:Avoid awaiting foreign Tasks",
    Justification = "Required to enable seamless LINQ integration with asynchronous operations.")]
  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Task<Result<T>> source,
    Func<T, Result<U>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return (await source).FlatMap(value => selector(value).Map(u => projector(value, u)));
  }

  [SuppressMessage(
    "Design",
    "VSTHRD200:Use \"Async\" suffix for async methods",
    Justification = "Need to be named SelectMany to work with LINQ query syntax")]
  [SuppressMessage(
    "Design",
    "VSTHRD003:Avoid awaiting foreign Tasks",
    Justification = "Required to enable seamless LINQ integration with asynchronous operations.")]
  public static async Task<Result<TResult>> SelectMany<T, U, TResult>(
    this Task<Result<T>> source,
    Func<T, Task<Result<U>>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await (await source).FlatMapAsync(async value => (await selector(value)).Map(u => projector(value, u)));
  }
}
