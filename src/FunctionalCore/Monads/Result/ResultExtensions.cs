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
}
