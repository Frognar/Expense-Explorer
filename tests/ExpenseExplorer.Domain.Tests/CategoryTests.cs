namespace ExpenseExplorer.Domain.Tests;

public class CategoryTests
{
  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ReturnsNoneWhenNameIsEmpty(string name)
  {
    Category.TryCreate(name)
      .Match(() => " [EMPTY] ", c => c.Name)
      .Should()
      .Be(" [EMPTY] ");
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Category.TryCreate(name)
      .Match(() => " [EMPTY] ", c => c.Name)
      .Should()
      .Be(name.Trim());
  }
}
