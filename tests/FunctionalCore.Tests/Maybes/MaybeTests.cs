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
  public void MapsSomeValue(int value)
  {
    var maybe = GetMaybe(value);

    var projected = maybe.Map(v => v * 2);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsSomeValueWhenSelectorIsAsync(int value)
  {
    var maybe = GetMaybe(value);

    var projected = await maybe.MapAsync(v => Task.FromResult(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsSomeValueWhenSourceIsAsync(int value)
  {
    var result = Task.FromResult(GetMaybe(value));

    var projected = await result.MapAsync(v => v * 2);

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsSomeValueWhenBothSourceAndSelectorAreAsync(int value)
  {
    var result = Task.FromResult(GetMaybe(value));

    var projected = await result.MapAsync(v => Task.FromResult(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public void FlatMapsSomeValue(int value)
  {
    var maybe = GetMaybe(value);

    var projected = maybe.FlatMap(v => Some.From(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task FlatMapsSuccessValueWhenSelectorIsAsync(int value)
  {
    var result = GetMaybe(value);

    var projected = await result.FlatMapAsync(v => Task.FromResult(Some.From(v * 2)));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task FlatMapsSuccessValueWhenSourceIsAsync(int value)
  {
    var result = Task.FromResult(GetMaybe(value));

    var projected = await result.FlatMapAsync(v => Some.From(v * 2));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task FlatMapsSuccessValueWhenBothSourceAndSelectorAreAsync(int value)
  {
    var result = Task.FromResult(GetMaybe(value));

    var projected = await result.FlatMapAsync(v => Task.FromResult(Some.From(v * 2)));

    AssertMaybe(projected, expectNone: value < 0, expectedValue: value * 2);
  }

  [Property]
  public void ChangeToResult(int value)
  {
    var maybe = GetMaybe(value);

    var result = maybe.ToResult(() => Failure.Validation("value", "Negative"));

    result.Match(_ => 0, s => s)
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
