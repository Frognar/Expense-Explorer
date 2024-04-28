namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public record Category
{
  private Category(string name)
  {
    EmptyCategoryNameException.ThrowIfEmpty(name);
    Name = name.Trim();
  }

  public string Name { get; }

  public static Category Create(string name)
  {
    return new Category(name);
  }

  public static Maybe<Category> TryCreate(string name)
  {
    return string.IsNullOrWhiteSpace(name)
      ? None.OfType<Category>()
      : Some.From(new Category(name));
  }
}
