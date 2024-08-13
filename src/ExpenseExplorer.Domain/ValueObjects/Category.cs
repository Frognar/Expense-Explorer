using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Category
{
  private Category(string name)
  {
    Name = name.Trim();
  }

  public string Name { get; }

  public static Maybe<Category> TryCreate(string name)
  {
    return !string.IsNullOrWhiteSpace(name)
      ? Some.With(new Category(name))
      : None.OfType<Category>();
  }
}
