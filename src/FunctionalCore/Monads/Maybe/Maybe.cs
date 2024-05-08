namespace FunctionalCore.Monads;

public readonly partial record struct Maybe<T>
{
  private readonly IMaybe _maybeType;

  private Maybe(IMaybe maybeType)
  {
    _maybeType = maybeType;
  }

  public TResult Match<TResult>(Func<TResult> none, Func<T, TResult> some)
  {
    ArgumentNullException.ThrowIfNull(none);
    ArgumentNullException.ThrowIfNull(some);
    return _maybeType switch
    {
      NoneType => none(),
      SomeType someType => some(someType.Value),
      _ => throw new InvalidOperationException("Unknown maybe type."),
    };
  }

  public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> none, Func<T, Task<TResult>> some)
  {
    ArgumentNullException.ThrowIfNull(none);
    ArgumentNullException.ThrowIfNull(some);
    return _maybeType switch
    {
      NoneType => await none().ConfigureAwait(false),
      SomeType someType => await some(someType.Value).ConfigureAwait(false),
      _ => throw new InvalidOperationException("Unknown maybe type."),
    };
  }

  public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Maybe<TResult>.None, v => Maybe<TResult>.Some(map(v)));
  }

  public async Task<Maybe<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return await MatchAsync(
      () => Task.FromResult(Maybe<TResult>.None()),
      async value => Maybe<TResult>.Some(await map(value)));
  }

  public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Maybe<TResult>.None, map);
  }

  public async Task<Maybe<TResult>> FlatMapAsync<TResult>(Func<T, Task<Maybe<TResult>>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return await MatchAsync(() => Task.FromResult(Maybe<TResult>.None()), map);
  }

  internal static Maybe<T> Some(T value) => new(new SomeType(value));

  internal static Maybe<T> None() => new(default(NoneType));
}
