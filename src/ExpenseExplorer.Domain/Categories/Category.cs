namespace ExpenseExplorer.Domain.Categories;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public sealed record Category
{
  private Category(
    Id id,
    ValueObjects.Category name,
    Description description,
    IEnumerable<Fact> unsavedChanges,
    Version version)
  {
    Id = id;
    Name = name;
    Description = description;
    UnsavedChanges = unsavedChanges;
    Version = version;
  }

  public Id Id { get; }

  public ValueObjects.Category Name { get; private init; }

  public Description Description { get; private init; }

  public IEnumerable<Fact> UnsavedChanges { get; private init; }

  public Version Version { get; private init; }
}
