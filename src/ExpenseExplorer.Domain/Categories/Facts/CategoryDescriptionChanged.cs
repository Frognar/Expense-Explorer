namespace ExpenseExplorer.Domain.Categories.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public sealed record CategoryDescriptionChanged(string Id, string Description) : Fact
{
  public static CategoryDescriptionChanged Create(Id id, Description description)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(description);
    return new CategoryDescriptionChanged(id.Value, description.Value);
  }
}
