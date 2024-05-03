namespace ExpenseExplorer.Domain.Tests;

public class ItemTests
{
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Item.TryCreate(name)
      .Match(() => " [EMPTY] ", i => i.Name)
      .Should()
      .Be(name.Trim());
  }

  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ReturnsNoneWhenNameIsEmpty(string name)
  {
    Item.TryCreate(name)
      .Match(() => " [EMPTY] ", i => i.Name)
      .Should()
      .Be(" [EMPTY] ");
  }
}
