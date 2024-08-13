using ExpenseExplorer.Tests.Common;

namespace ExpenseExplorer.Domain.Tests;

public class IdTests
{
  [Fact]
  public void IdsAreConsiderEqualsWhenTheyHaveTheSameValue()
  {
    Assert.Equal(Id.TryCreate("123").ForceValue(), Id.TryCreate("123 ").ForceValue());
    Assert.Equal(Id.TryCreate("123").ForceValue(), Id.TryCreate(" 123").ForceValue());
    Assert.Equal("123", Id.TryCreate("123").ForceValue().Value);
    Assert.NotEqual(Id.TryCreate("123").ForceValue(), Id.TryCreate("234").ForceValue());
    Assert.NotEqual(Id.TryCreate("123").ForceValue(), Id.TryCreate("234").ForceValue());
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
