using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Categories.Facts;

public sealed record CategoryRenamed(string Id, string Name) : Fact
{
  public static CategoryRenamed Create(Id id, ValueObjects.Category name)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(name);
    return new CategoryRenamed(id.Value, name.Name);
  }
}
