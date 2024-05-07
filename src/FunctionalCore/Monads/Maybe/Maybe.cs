namespace FunctionalCore.Monads;

public class Maybe<T>
{
  private readonly IMaybe _maybe;

  private Maybe(IMaybe maybe)
  {
    _maybe = maybe;
  }

  private interface IMaybe;

  public TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome)
  {
    ArgumentNullException.ThrowIfNull(onNone);
    ArgumentNullException.ThrowIfNull(onSome);
    return _maybe switch
    {
      NoneType => onNone(),
      SomeType some => onSome(some.Value),
      _ => throw new InvalidOperationException("Unknown maybe type."),
    };
  }

  public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Maybe<TResult>.None, v => Maybe<TResult>.Some(map(v)));
  }

  public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> map)
  {
    ArgumentNullException.ThrowIfNull(map);
    return Match(Maybe<TResult>.None, map);
  }

  internal static Maybe<T> Some(T value) => new(new SomeType(value));

  internal static Maybe<T> None() => new(default(NoneType));

  private readonly record struct SomeType(T Value) : IMaybe;

  private readonly record struct NoneType : IMaybe;
}
