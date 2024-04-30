namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.ValueObjects;

public class VersionTests
{
  [Fact]
  public void MaxByDefault()
  {
    Version version = Version.New();
    version.Value.Should().Be(ulong.MaxValue);
  }
}
