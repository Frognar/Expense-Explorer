namespace ExpenseExplorer.Application.Tests;

using System.Globalization;
using ExpenseExplorer.Application.Monads;

public class EitherTests
{
  [Property]
  public void CanCreateLeft(int left)
  {
    var either = Either<int, string>.Left(left);
    either.Should().NotBeNull();
  }

  [Property]
  public void CanCreateRight(NonEmptyString right)
  {
    var either = Either<int, string>.Right(right.Item);
    either.Should().NotBeNull();
  }

  [Property]
  public void MatchesCorrectly(int value)
  {
    var either = value < 0
      ? Either<string, int>.Left("Negative")
      : Either<string, int>.Right(value);

    either.Match(left => left, right => right.ToString(CultureInfo.InvariantCulture))
      .Should().Be(value < 0 ? "Negative" : value.ToString(CultureInfo.InvariantCulture));
  }
}
