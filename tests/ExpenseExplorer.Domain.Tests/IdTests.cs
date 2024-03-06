using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Tests;

public class IdTests {
  [Fact]
  public void IdsAreConsiderEqualsWhenTheyHaveTheSameValue() {
    Assert.Equal(new Id(""), new Id(""));
    Assert.Equal(new Id("123"), new Id("123 "));
    Assert.Equal(new Id("123"), new Id(" 123"));
    Assert.Equal("123", new Id("123").Value);
    Assert.NotEqual(new Id("123"), new Id("234"));
    Assert.NotEqual(new Id("123"), new Id("234"));
  }
}
