namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.ValueObjects;

public class IdTests
{
  [Fact]
  public void IdsAreConsiderEqualsWhenTheyHaveTheSameValue()
  {
    Assert.Equal(new Id(string.Empty), new Id(string.Empty));
    Assert.Equal(new Id("123"), new Id("123 "));
    Assert.Equal(new Id("123"), new Id(" 123"));
    Assert.Equal("123", new Id("123").Value);
    Assert.NotEqual(new Id("123"), new Id("234"));
    Assert.NotEqual(new Id("123"), new Id("234"));
  }

  [Fact]
  public void IdsAreUnique()
  {
    List<Id> ids = GenerateIds(1000);
    HashSet<Id> idSet = ids.ToHashSet();
    Assert.Equal(idSet.Count, ids.Count);
  }

  private static List<Id> GenerateIds(int n)
  {
    return Enumerable.Range(0, n).Select(_ => Id.Unique()).ToList();
  }
}
