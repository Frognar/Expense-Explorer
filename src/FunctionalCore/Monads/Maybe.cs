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

  internal static Maybe<T> Some(T value) => new(new SomeValue(value));

  internal static Maybe<T> None() => new(default(NoneValue));

  private readonly record struct SomeValue(T Value) : IMaybe
  {
    public TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome) => onSome(Value);
  }

  private readonly record struct NoneValue : IMaybe
  {
    public TResult Match<TResult>(Func<TResult> onNone, Func<T, TResult> onSome) => onNone();
  }
}
