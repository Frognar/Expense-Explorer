namespace ExpenseExplorer.Domain.Tests;

public class DescriptionTests
{
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void DescriptionIsTrimmed(string description)
  {
    Description.TryCreate(description)
      .Match(() => " [EMPTY] ", d => d.Value)
      .Should()
      .Be(description.Trim());
  }

  [Property]
  public void IsEmptyWhenCreatedWithNull()
  {
    Description.TryCreate(null)
      .Match(() => " [EMPTY] ", d => d.Value)
      .Should()
      .BeEmpty();
  }
}
