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

  [Property]
  public void MatchesCorrectly(int value)
  {
    Maybe<int> maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    maybe.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value);
  }
}
