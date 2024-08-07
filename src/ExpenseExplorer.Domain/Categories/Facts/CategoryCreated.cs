namespace ExpenseExplorer.Domain.Categories.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public sealed record CategoryCreated(string Id, string Name, string Description) : Fact
{
  public static CategoryCreated Create(Id id, ValueObjects.Category name, Description description)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(name);
    ArgumentNullException.ThrowIfNull(description);
    return new CategoryCreated(id.Value, name.Name, description.Value);
  }
}
