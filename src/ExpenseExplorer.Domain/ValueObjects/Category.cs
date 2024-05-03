namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

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
      ? Some.From(new Category(name))
      : None.OfType<Category>();
  }
}
