namespace FunctionalCore.Tests;

public class EitherTests
{
  [Property]
  public void CanCreateLeft(int left)
  {
    var either = Left.From<int, string>(left);
    either.Should().NotBeNull();
  }

  [Property]
  public void CanCreateRight(NonEmptyString right)
  {
    var either = Right.From<int, string>(right.Item);
    either.Should().NotBeNull();
  }

  [Property]
  public void MatchesCorrectly(int value)
  {
    var either = value < 0
      ? Left.From<string, int>("Negative")
      : Right.From<string, int>(value);

    either.Match(left => left, right => right.ToString(CultureInfo.InvariantCulture))
      .Should()
      .Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }

  [Property]
  public void MapsLeftWhenLeft(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either.MapLeft(l => l.ToString(CultureInfo.InvariantCulture));

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value.ToString(CultureInfo.InvariantCulture) : "Negative");
  }

  [Property]
  public void MapsRightWhenRight(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either.MapRight(r => r.Length);

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value : "Negative".Length);
  }

  [Property]
  public void MapsRightWithQuerySyntax(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected =
      from r in either
      select r.Length;

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value : "Negative".Length);
  }

  [Property]
  public void MapsEitherLeftOrRight(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either.MapBoth(
      l => l.ToString(CultureInfo.InvariantCulture),
      r => r.Length);

    projected.Match(l => l.Length, r => r)
      .Should()
      .Be(value < 0 ? value.ToString(CultureInfo.InvariantCulture).Length : "Negative".Length);
  }

  [Property]
  public void FlatMapsLeftWhenLeft(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either
      .FlatMapLeft(l => Right.From<string, string>(l.ToString(CultureInfo.InvariantCulture)));

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value.ToString(CultureInfo.InvariantCulture) : "Negative");
  }

  [Property]
  public void FlatMapsRightWhenRight(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either
      .FlatMapRight(r => Left.From<int, int>(r.Length));

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value : "Negative".Length);
  }

  [Property]
  public void FlatMapsEitherLeftOrRight(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either
      .FlatMapBoth(Right.From<string, int>, Left.From<string, int>);

    projected.Match(l => l.Length, r => r)
      .Should()
      .Be(value < 0 ? value : "Negative".Length);
  }

  [Property]
  public void FlatMapsRightWithQuerySyntax(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected =
      from r in either
      from r1 in either
      select (r + r1).Length;

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value : "NegativeNegative".Length);
  }

  [Property]
  public void ChangeToResult(int value)
  {
    var either = value < 0
      ? Left.From<Failure, int>(new TestFailure("Negative"))
      : Right.From<Failure, int>(value);

    var result = either.ToResult();

    result.Match(f => f.Message.Length, s => s)
      .Should()
      .Be(value < 0 ? "Negative".Length : value);
  }

  private sealed record TestFailure(string Message) : Failure(Message);
}
