namespace FunctionalCore.Tests.Results;

using static HelperMethods;

public class ResultsTests
{
  [Property]
  public void MatchesCorrectly(int value)
  {
    AssertResult(GetResult(value), expectFailure: value < 0, expectedValue: value);
  }

  [Property]
  public void MapsSuccessValue(int value)
  {
    var result = GetResult(value);

    var projected = result.Map(v => v * 2);

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsSuccessValueWhenSelectorIsAsync(int value)
  {
    var result = GetResult(value);

    var projected = await result.MapAsync(v => Task.FromResult(v * 2));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsSuccessValueWhenSourceIsAsync(int value)
  {
    var result = Task.FromResult(GetResult(value));

    var projected = await result.MapAsync(v => v * 2);

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task MapsSuccessValueWhenBothSourceAndSelectorAreAsync(int value)
  {
    var result = Task.FromResult(GetResult(value));

    var projected = await result.MapAsync(v => Task.FromResult(v * 2));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public void FlatMapsSuccessValue(int value)
  {
    var result = GetResult(value);

    var projected = result.FlatMap(v => Success.From(v * 2));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task FlatMapsSuccessValueWhenSelectorIsAsync(int value)
  {
    var result = GetResult(value);

    var projected = await result.FlatMapAsync(v => Task.FromResult(Success.From(v * 2)));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task FlatMapsSuccessValueWhenSourceIsAsync(int value)
  {
    var result = Task.FromResult(GetResult(value));

    var projected = await result.FlatMapAsync(v => Success.From(v * 2));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }

  [Property]
  public async Task FlatMapsSuccessValueWhenBothSourceAndSelectorAreAsync(int value)
  {
    var result = Task.FromResult(GetResult(value));

    var projected = await result.FlatMapAsync(v => Task.FromResult(Success.From(v * 2)));

    AssertResult(projected, expectFailure: value < 0, expectedValue: value * 2);
  }
}
