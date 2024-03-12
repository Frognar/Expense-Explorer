namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;

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
}
