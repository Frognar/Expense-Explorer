namespace FunctionalCore.Monads;

public readonly partial record struct Maybe<T>
{
  private interface IMaybe;

  private readonly record struct SomeType(T Value) : IMaybe;

  private readonly record struct NoneType : IMaybe;
}
