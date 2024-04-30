namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public readonly record struct Item(string Name)
{
  private readonly string _name = TrimOrThrow(Name);

  public string Name
  {
    get => _name;
    init => _name = TrimOrThrow(value);
  }

  public static Item Create(string name)
  {
    return new Item(name);
  }

  public static Maybe<Item> TryCreate(string name)
  {
    return string.IsNullOrWhiteSpace(name)
      ? None.OfType<Item>()
      : Some.From(new Item(name));
  }

  private static string TrimOrThrow(string name)
  {
    ArgumentNullException.ThrowIfNull(name);
    return IsValid(name) ? name.Trim() : throw new EmptyItemNameException();
  }

  private static bool IsValid(string name)
  {
    return !string.IsNullOrWhiteSpace(name);
  }
}
