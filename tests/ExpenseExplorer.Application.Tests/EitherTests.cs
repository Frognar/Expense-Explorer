namespace ExpenseExplorer.Application.Tests;

using System.Globalization;
using ExpenseExplorer.Application.Monads;

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
  public void ProjectsLeftWhenLeft(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either.MapLeft(s => s.ToString(CultureInfo.InvariantCulture));

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value.ToString(CultureInfo.InvariantCulture) : "Negative");
  }

  [Property]
  public void ProjectsRightWhenRight(int value)
  {
    var either = value < 0
      ? Left.From<int, string>(value)
      : Right.From<int, string>("Negative");

    var projected = either.MapRight(s => s.Length);

    projected.Match(l => l, r => r)
      .Should()
      .Be(value < 0 ? value : "Negative".Length);
  }

  [Property]
  public void ProjectsRightWithQuerySyntax(int value)
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
}
