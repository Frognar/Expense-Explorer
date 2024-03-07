namespace ExpenseExplorer.Domain.ValueObjects;

public record Store
{
  private Store(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Name = name.Trim();
  }

  public string Name { get; }

  public static Store Create(string name)
  {
    return new Store(name);
  }
}
