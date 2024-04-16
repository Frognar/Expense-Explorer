namespace FunctionalCore.Monads;

public class Maybe<T>
{
#pragma warning disable S4487
  private readonly T _value;
#pragma warning restore S4487

  private Maybe(T value)
  {
    _value = value;
  }

  internal static Maybe<T> Some(T value) => new(value);

  internal static Maybe<T> None() => new(default!);
}
