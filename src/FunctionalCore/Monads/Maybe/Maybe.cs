namespace FunctionalCore.Monads;

public class Maybe<T>
{
  private readonly IMaybe _maybe;

  private Maybe(IMaybe maybe)
  {
    _maybe = maybe;
  }

  private interface IMaybe
  {
    TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome);
  }

  public TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome)
  {
    ArgumentNullException.ThrowIfNull(onNone);
    ArgumentNullException.ThrowIfNull(onSome);
    return _maybe.Match(onNone, onSome);
  }

  public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Maybe<TResult>.None, v => Maybe<TResult>.Some(map(v)));
  }

  public Maybe<TResult> Select<TResult>(Func<T, TResult> selector) => Map(selector);

  public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Maybe<TResult>.None, map);
  }

  public Maybe<TResult> SelectMany<TResult, T1>(Func<T, Maybe<T1>> selector, Func<T, T1, TResult> projector)
  {
    ArgumentNullException.ThrowIfNull(selector);
    ArgumentNullException.ThrowIfNull(projector);
    return FlatMap(v => selector(v).Map(i => projector(v, i)));
  }

  public T OrElse(Func<T> fallback)
  {
    ArgumentNullException.ThrowIfNull(fallback);
    return Match(fallback, value => value);
  }

  internal static Maybe<T> Some(T value) => new(new SomeValue(value));

  internal static Maybe<T> None() => new(default(NoneValue));

  private readonly record struct SomeValue(T Value) : IMaybe
  {
    private T Value { get; } = Value;

    public TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome) => onSome(Value);
  }

  private readonly record struct NoneValue : IMaybe
  {
    public TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome) => onNone();
  }
}
