namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Exceptions;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Strings;

public class StoreTests
{
  [Property(Arbitrary = [typeof(EmptyStringGenerator)])]
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
}
