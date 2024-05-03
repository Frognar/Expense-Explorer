namespace ExpenseExplorer.Domain.Tests;

public class StoreTests
{
  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmpty(string name)
  {
    Action act = () => _ = Store.Create(name);
    act.Should().Throw<EmptyStoreNameException>();
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name)
  {
    Store.Create(name).Name.Should().Be(name.Trim());
  }

  [Property(Arbitrary = [typeof(EmptyOrWhiteSpaceStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmptyWithRecordSyntax(string name)
  {
    Action act = () => _ = Store.Create("name") with { Name = name };
    act.Should().Throw<EmptyStoreNameException>();
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmedWithRecordSyntax(string name)
  {
    Store store = Store.Create("name") with { Name = name };
    store.Name.Should().Be(name.Trim());
  }
}
