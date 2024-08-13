namespace ExpenseExplorer.Domain.Tests;

public class VersionTests
{
  [Fact]
  public void MaxByDefault()
  {
    Version version = Version.New();
    version.Value.Should().Be(ulong.MaxValue);
  }
}
