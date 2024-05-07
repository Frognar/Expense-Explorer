namespace FunctionalCore.Monads;

using System.Diagnostics.CodeAnalysis;
using FunctionalCore.Failures;

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

  public static Maybe<TResult> Select<T, TResult>(
    this Maybe<T> source,
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(selector);
    return source.Map(selector);
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
