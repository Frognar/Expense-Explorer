namespace FunctionalCore.Monads;

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods - Need to be named Select/SelectMany to work with LINQ query syntax
public readonly partial record struct Maybe<T>
{
  public Maybe<TResult> Select<TResult>(
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return Map(selector);
  }

  public async Task<Maybe<TResult>> Select<TResult>(
    Func<T, Task<TResult>> selector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return await MapAsync(selector);
  }

  public Maybe<TResult> SelectMany<TResult, T1>(
    Func<T, Maybe<T1>> selector,
    Func<T, T1, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return FlatMap(v => selector(v).Map(i => projector(v, i)));
  }

  public async Task<Maybe<TResult>> SelectMany<U, TResult>(
    Func<T, Task<Maybe<U>>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await FlatMapAsync(async value => (await selector(value)).Map(u => projector(value, u)));
  }

  public async Task<Maybe<TResult>> SelectMany<U, TResult>(
    Func<T, Maybe<U>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public async Task<Maybe<TResult>> SelectMany<U, TResult>(
    Func<T, Task<Maybe<U>>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }
}
#pragma warning restore VSTHRD200
