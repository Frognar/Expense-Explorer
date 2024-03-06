using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Tests;

public class StoreTests {
  [Property(Arbitrary = [typeof(EmptyStringGenerator)])]
  public void ThrowsExceptionWhenNameIsEmpty(string name) {
    Action act = () => _ = Store.Create(name);
    act.Should().Throw<ArgumentException>();
  }

  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void NameIsTrimmed(string name) {
    Store store = Store.Create(name);
    store.Name.Should().Be(name.Trim());
  }
}