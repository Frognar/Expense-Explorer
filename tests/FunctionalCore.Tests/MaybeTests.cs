namespace FunctionalCore.Tests;

using FunctionalCore.Monads;

public class MaybeTests
{
  [Property]
  public void CanCreateSome(int value)
  {
    Maybe<int> maybe = Some.From(value);
    maybe.Should().NotBeNull();
  }
}
