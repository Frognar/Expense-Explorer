namespace ExpenseExplorer.Domain.Tests;

public class DescriptionTests
{
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void DescriptionIsTrimmed(string description)
  {
    Description.Create(description).Value.Should().Be(description.Trim());
  }

  [Property]
  public void IsEmptyWhenCreatedWithNull()
  {
    Description.Create(null).Value.Should().BeEmpty();
  }
}
