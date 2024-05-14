namespace ExpenseExplorer.Domain.Tests;

public class SourceTests
{
  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ReturnsNoneWhenNameIsEmpty(string name)
  {
    Source.TryCreate(name)
      .Match(() => " [EMPTY] ", c => c.Name)
      .Should()
      .Be(" [EMPTY] ");
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Source.TryCreate(name)
      .Match(() => " [EMPTY] ", c => c.Name)
      .Should()
      .Be(name.Trim());
  }
}
