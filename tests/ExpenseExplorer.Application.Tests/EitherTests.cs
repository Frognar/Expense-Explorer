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
}
