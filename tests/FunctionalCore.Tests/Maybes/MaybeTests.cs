namespace FunctionalCore.Tests.Maybes;

using static HelperMethods;

public class MaybeTests
{
  [Property]
  public void MatchesCorrectly(int value)
  {
    AssertMaybe(GetMaybe(value), expectNone: value < 0, expectedValue: value);
  }

  [Property]
  public void MapsWhenSome(int value)
  {
    var maybe = GetMaybe(value);

    var projected = maybe.Map(v => v * 2);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsAsyncWhenSome(int value)
  {
    var maybe = GetMaybe(value);

    var projected = await maybe.MapAsync(v => Task.FromResult(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public void MapsWithQuerySyntax(int value)
  {
    var maybe = GetMaybe(value);

    var projected =
      from v in maybe
      select v * 2;

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public void FlatMapsWhenSome(int value)
  {
    var maybe = GetMaybe(value);

    var projected = maybe.FlatMap(v => Some.From(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task FlatMapsAsyncWhenSome(int value)
  {
    var maybe = GetMaybe(value);

    var projected = await maybe.FlatMapAsync(v => Task.FromResult(Some.From(v * 2)));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public void FlatMapsWithQuerySyntax(int value)
  {
    var maybe = GetMaybe(value);

    var projected =
      from v in maybe
      from v1 in maybe
      select v + v1;

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public async Task FlatMapsAsyncWithQuerySyntax(int value)
  {
    var maybe = GetMaybe(value);

    var projected = await (
      from v in maybe
      from v1 in Task.FromResult(maybe)
      select v + v1);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value + value);
  }

  [Property]
  public void ChangeToResult(int value)
  {
    var maybe = GetMaybe(value);

    var result = maybe.ToResult(() => Failure.Validation("value", "Negative"));

    result.Match(f => f.Match((_, _) => 0, (_, _) => 0, (_, _) => 0), s => s)
      .Should()
      .Be(value < 0 ? 0 : value);
  }

  [Property]
  public void ReturnsFallbackValueWhenNone(int value)
  {
    GetMaybe(value)
      .OrElse(() => -1)
      .Should()
      .Be(value < 0 ? -1 : value);
  }
}
