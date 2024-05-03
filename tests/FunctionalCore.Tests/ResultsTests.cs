namespace FunctionalCore.Tests;

public class ResultsTests
{
  [Property]
  public void CanCreateSuccess(int value)
  {
    var result = Success.From(value);
    result.Should().NotBeNull();
  }

  [Fact]
  public void CanCreateFailure()
  {
    var result = Fail.OfType<int>(new TestFailure("Negative"));
    result.Should().NotBeNull();
  }

  [Property]
  public void MatchesCorrectly(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    result.Match(failure => failure.Message, v => v.ToString(CultureInfo.InvariantCulture))
      .Should()
      .Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public void MapsWhenSuccess(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    var projected = result.Map(v => v.ToString(CultureInfo.InvariantCulture));

    projected.Match(failure => failure.Message, r => r)
      .Should()
      .Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public async Task MapsAsyncWhenSuccess(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    var projected = await result.MapAsync(v => Task.FromResult(v.ToString(CultureInfo.InvariantCulture)));

    projected.Match(failure => failure.Message, r => r)
      .Should()
      .Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public void MapsWithQuerySyntax(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    var projected =
      from v in result
      select v.ToString(CultureInfo.InvariantCulture);

    projected.Match(failure => failure.Message, r => r)
      .Should()
      .Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public void FlatMapsWhenSuccess(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    var projected = result
      .FlatMap(v => Success.From(v.ToString(CultureInfo.InvariantCulture)));

    projected.Match(failure => failure.Message, r => r)
      .Should()
      .Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public async Task FlatMapsAsyncWhenSuccess(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    var projected = await result
      .FlatMapAsync(v => Task.FromResult(Success.From(v.ToString(CultureInfo.InvariantCulture))));

    projected.Match(failure => failure.Message, r => r)
      .Should()
      .Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public void FlatMapsWithQuerySyntax(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    var projected =
      from r in result
      from r1 in result
      select r + r1;

    projected.Match(failure => failure.Message, r => r.ToString(CultureInfo.InvariantCulture))
      .Should()
      .Be(value < 0 ? "Negative" : (value * 2).ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public async Task FlatMapsAsyncWithQuerySyntax(int value)
  {
    var result = value < 0
      ? Fail.OfType<int>(new TestFailure("Negative"))
      : Success.From(value);

    var projected = await (
      from r0 in result
      from r1 in Task.FromResult(result)
      from r2 in result
      from r3 in Task.FromResult(result)
      from r4 in result
      select r0 + r1 + r2 + r3 + r4);

    projected.Match(failure => failure.Message, r => r.ToString(CultureInfo.InvariantCulture))
      .Should()
      .Be(value < 0 ? "Negative" : (value * 5).ToString(CultureInfo.InvariantCulture));
  }

  private sealed record TestFailure(string Message) : Failure(Message);
}
