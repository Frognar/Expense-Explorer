namespace FunctionalCore.Tests;

using FunctionalCore.Monads;

public class MaybeTests
{
  [Property]
  public void CanCreateSome(int value)
  {
    var maybe = Some.From(value);
    maybe.Should().NotBeNull();
  }

  [Fact]
  public void CanCreateNone()
  {
    var maybe = None.OfType<int>();
    maybe.Should().NotBeNull();
  }

  [Property]
  public void MatchesCorrectly(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    maybe.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value);
  }

  [Property]
  public void MapsWhenSome(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    var projected = maybe.Map(v => v * 2);

    projected.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value * 2);
  }

  [Property]
  public async Task MapsAsyncWhenSome(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    var projected = await maybe.MapAsync(v => Task.FromResult(v * 2));

    projected.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value * 2);
  }

  [Property]
  public void MapsWithQuerySyntax(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    var projected =
      from v in maybe
      select v * 2;

    projected.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value * 2);
  }

  [Property]
  public void FlatMapsWhenSome(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    var projected = maybe.FlatMap(v => Some.From(v * 2));

    projected.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value * 2);
  }

  [Property]
  public async Task FlatMapsAsyncWhenSome(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    var projected = await maybe.FlatMapAsync(v => Task.FromResult(Some.From(v * 2)));

    projected.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value * 2);
  }

  [Property]
  public void FlatMapsWithQuerySyntax(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    var projected =
      from v in maybe
      from v1 in maybe
      select v + v1;

    projected.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value * 2);
  }

  [Property]
  public async Task FlatMapsAsyncWithQuerySyntax(int value)
  {
    var maybe = value < 0
      ? None.OfType<int>()
      : Some.From(value);

    var projected = await (
      from v in maybe
      from v1 in Task.FromResult(maybe)
      select v + v1);

    projected.Match(() => -1, v => v)
      .Should()
      .Be(value < 0 ? -1 : value * 2);
  }
}
