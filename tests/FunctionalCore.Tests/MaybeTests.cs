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

  [Fact]
  public void CanCreateNone()
  {
    Maybe<int> maybe = None.OfType<int>();
    maybe.Should().NotBeNull();
  }
}
