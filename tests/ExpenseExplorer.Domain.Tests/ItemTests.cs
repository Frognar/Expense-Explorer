namespace ExpenseExplorer.Domain.Tests;

public class ItemTests
{
  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmpty(string name)
  {
    Action act = () => _ = Item.Create(name);
    act.Should().Throw<EmptyItemNameException>();
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Item.Create(name).Name.Should().Be(name.Trim());
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmedWithRecordSyntax(string name)
  {
    Item item = Item.Create("123") with { Name = name };
    item.Name.Should().Be(name.Trim());
  }

  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmptyWithRecordSyntax(string name)
  {
    Action act = () => _ = Item.Create("123") with { Name = name };
    act.Should().Throw<EmptyItemNameException>();
  }
}
