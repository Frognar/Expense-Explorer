namespace FunctionalCore.Monads;

public class Maybe<T>
{
  private readonly bool _hasValue;
  private readonly T _value;

  private Maybe(T value, bool hasValue)
  {
    _value = value;
    _hasValue = hasValue;
  }

  public TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome)
  {
    ArgumentNullException.ThrowIfNull(onNone);
    ArgumentNullException.ThrowIfNull(onSome);
    return _hasValue ? onSome(_value) : onNone();
  }

  internal static Maybe<T> Some(T value) => new(value, true);

  internal static Maybe<T> None() => new(default!, false);
}
