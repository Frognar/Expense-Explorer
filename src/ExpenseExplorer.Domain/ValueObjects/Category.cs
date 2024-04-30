namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public readonly record struct Category(string Name)
{
  private readonly string _name = TrimOrThrow(Name);

  public string Name
  {
    get => _name;
    init => _name = TrimOrThrow(value);
  }

  public static Category Create(string name)
  {
    return new Category(name);
  }

  public static Maybe<Category> TryCreate(string name)
  {
    return IsValid(name)
      ? Some.From(new Category(name))
      : None.OfType<Category>();
  }

  private static string TrimOrThrow(string name)
  {
    ArgumentNullException.ThrowIfNull(name);
    return IsValid(name) ? name.Trim() : throw new EmptyCategoryNameException();
  }

  private static bool IsValid(string name)
  {
    return !string.IsNullOrWhiteSpace(name);
  }
}
