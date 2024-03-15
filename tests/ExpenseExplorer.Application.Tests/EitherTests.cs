namespace ExpenseExplorer.Application.Tests;

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
}
