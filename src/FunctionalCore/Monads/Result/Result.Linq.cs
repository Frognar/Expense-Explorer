namespace FunctionalCore.Monads;

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods - Need to be named Select/SelectMany to work with LINQ query syntax
public readonly partial record struct Result<T>
{
  public Result<TResult> Select<TResult>(
    Func<T, TResult> selector)
  {
    ArgumentNullException.ThrowIfNull(this);
    ArgumentNullException.ThrowIfNull(selector);
    return Map(selector);
  }

  public async Task<Result<TResult>> Select<TResult>(
    Func<T, Task<TResult>> selector)
  {
    ArgumentNullException.ThrowIfNull(this);
    ArgumentNullException.ThrowIfNull(selector);
    return await MapAsync(selector);
  }

  public Result<TResult> SelectMany<U, TResult>(
    Func<T, Result<U>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(this);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return FlatMap(value => selector(value).Map(u => projector(value, u)));
  }

  public async Task<Result<TResult>> SelectMany<U, TResult>(
    Func<T, Task<Result<U>>> selector,
    Func<T, U, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(this);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public async Task<Result<TResult>> SelectMany<U, TResult>(
    Func<T, Result<U>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(this);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }

  public async Task<Result<TResult>> SelectMany<U, TResult>(
    Func<T, Task<Result<U>>> selector,
    Func<T, U, Task<TResult>> projector)
  {
    ArgumentNullException.ThrowIfNull(this);
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return await FlatMapAsync(async value => await selector(value).MapAsync(u => projector(value, u)));
  }
}
#pragma warning restore VSTHRD200
