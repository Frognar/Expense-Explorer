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

  [Property]
  public void IncrementingVersion(ulong value)
  {
    Version version = Version.Create(value);

    Version next = version.Next();

    next.Value.Should().Be(value + 1);
  }
}
