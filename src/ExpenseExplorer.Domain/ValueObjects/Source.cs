namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

public readonly record struct Source
{
  private Source(string name)
  {
    Name = name.Trim();
  }

  public string Name { get; }

  public static Maybe<Source> TryCreate(string name)
  {
    return !string.IsNullOrWhiteSpace(name)
      ? Some.From(new Source(name))
      : None.OfType<Source>();
  }
}
