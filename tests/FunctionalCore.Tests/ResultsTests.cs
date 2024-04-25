namespace FunctionalCore.Tests;

using System.Globalization;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

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

  private sealed record TestFailure(string Message) : Failure(Message);
}
