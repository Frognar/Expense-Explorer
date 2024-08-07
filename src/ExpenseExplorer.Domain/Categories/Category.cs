namespace ExpenseExplorer.Domain.Categories;

using ExpenseExplorer.Domain.Categories.Facts;
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

  public static Category New(ValueObjects.Category name, Description description)
  {
    Id id = Id.Unique();
    Fact categoryCreated = CategoryCreated.Create(id, name, description);
    return new Category(id, name, description, [categoryCreated], Version.New());
  }

  public Category Rename(ValueObjects.Category name)
  {
    Fact categoryRenamed = CategoryRenamed.Create(Id, name);
    return this with { Name = name, UnsavedChanges = UnsavedChanges.Append(categoryRenamed).ToList() };
  }

  public Category ChangeDescription(Description description)
  {
    Fact categoryDescriptionChanged = CategoryDescriptionChanged.Create(Id, description);
    return this with { Description = description, UnsavedChanges = UnsavedChanges.Append(categoryDescriptionChanged).ToList() };
  }
}
