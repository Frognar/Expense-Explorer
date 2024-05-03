namespace ExpenseExplorer.Domain.Tests;

public class StoreTests
{
  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ReturnsNoneWhenNameIsEmpty(string name)
  {
    Store.TryCreate(name)
      .Match(() => " [EMPTY] ", s => s.Name)
      .Should()
      .Be(" [EMPTY] ");
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Store.TryCreate(name)
      .Match(() => " [EMPTY] ", s => s.Name)
      .Should()
      .Be(name.Trim());
  }
}
