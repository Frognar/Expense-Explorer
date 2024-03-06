using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Tests;

public class StoreTests {
  [Property(Arbitrary = [typeof(NonEmptyStringGenerator)])]
  public void StoreNameIsSetDuringConstruction(string name) {
    Store store = new(name);
    store.Name.Should().Be(name);
  }
}
