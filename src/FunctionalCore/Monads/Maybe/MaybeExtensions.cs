namespace FunctionalCore.Monads;

using System.Diagnostics.CodeAnalysis;

public static class MaybeExtensions
{
  public static async Task<Maybe<TResult>> MapAsync<T, TResult>(this Maybe<T> source, Func<T, Task<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await source.Match(
      () => Task.FromResult(None.OfType<TResult>()),
      async value => Some.From(await map(value)));
  }

  public static async Task<Maybe<TResult>> FlatMapAsync<T, TResult>(
    this Maybe<T> source,
    Func<T, Task<Maybe<TResult>>> map)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(map);
    return await source.Match(
      () => Task.FromResult(None.OfType<TResult>()),
      async value => await map(value));
  }

  [SuppressMessage(
    "Design",
    "VSTHRD200:Use \"Async\" suffix for async methods",
    Justification = "Need to be named SelectMany to work with LINQ query syntax")]
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
}
