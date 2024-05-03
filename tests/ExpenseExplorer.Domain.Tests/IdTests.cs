namespace ExpenseExplorer.Domain.Tests;

public class IdTests
{
  [Fact]
  public void IdsAreConsiderEqualsWhenTheyHaveTheSameValue()
  {
    Assert.Equal(Id.Create(string.Empty), Id.Create(string.Empty));
    Assert.Equal(Id.Create("123"), Id.Create("123 "));
    Assert.Equal(Id.Create("123"), Id.Create(" 123"));
    Assert.Equal("123", Id.Create("123").Value);
    Assert.NotEqual(Id.Create("123"), Id.Create("234"));
    Assert.NotEqual(Id.Create("123"), Id.Create("234"));
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
